using Autofac;
using FlowBot.Common.Exceptions;
using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Providers;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Models;
using FlowBot.Services.Extensions;
using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.DurableInstancing;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;

namespace FlowBot.Services
{
    public class WorkflowService : IWorkflowService
    {
        private static readonly XName WorkflowHostTypePropertyName = XNamespace.Get("urn:schemas-microsoft-com:System.Activities/4.0/properties").GetName("WorkflowHostType");
        private static string s_workflowInstanceStoreConnectionString = ConfigurationManager.ConnectionStrings["WorkflowInstanceStore"].ConnectionString;
        private const string WorkflowFileExtension = ".xaml";
        private ILifetimeScopeProvider _lifetimeScopeProvider;
        private IDataService _dataService;
        private InstanceStore _instanceStore = null;
        private InstanceView _instanceView = null;
        private string _workflowRootDirectory;
        private static readonly Dictionary<Type,Type> s_persistableServices = new Dictionary<Type,Type>();
        public static void RegisterPersistableService<T,AsT>()
        {
            if ( typeof(T).IsAssignableTo<IPersistable>())
            {
                s_persistableServices.Add(typeof(AsT),typeof(T));
            }
            else
            {
                throw new ArgumentException($"{typeof(T)} does not implement {typeof(IPersistable)}", "serviceType");
            }
        }
        public WorkflowService(ILifetimeScopeProvider lifetimeScopeProvider, IDataService dataService)
        {
            _lifetimeScopeProvider = lifetimeScopeProvider;
            _dataService = dataService;
        }
        public void SetWorkflowRootDirectory(string workflowRootDirectory)
        {
            _workflowRootDirectory = workflowRootDirectory;
        }
        private static XName CreateHostTypeName(WorkflowIdentity workflowIdentity)
        {
            return XName.Get(workflowIdentity.Package + ":" + workflowIdentity.Name + ":" + workflowIdentity.Version, typeof(WorkflowService).FullName);
        }

        private static InstanceHandle CreateInstanceStoreOwnerHandle(InstanceStore store, XName wfHostTypeName)
        {

            InstanceHandle ownerHandle = store.CreateInstanceHandle();

            CreateWorkflowOwnerCommand ownerCommand = new CreateWorkflowOwnerCommand()
            {

                InstanceOwnerMetadata = {

                { WorkflowHostTypePropertyName, new InstanceValue(wfHostTypeName) }

            }

            };

            store.DefaultInstanceOwner = store.Execute(ownerHandle, ownerCommand, TimeSpan.FromSeconds(30)).InstanceOwner;

            return ownerHandle;

        }

        protected InstanceStore GetInstanceStore(WorkflowApplication workflowApplication, ILifetimeScope workflowScope)
        {
            SqlWorkflowInstanceStore instanceStore = new SqlWorkflowInstanceStore(s_workflowInstanceStoreConnectionString);
            List<XName> variantProperties = new List<XName>();
            foreach( var persistableServiceEntry in s_persistableServices)
            {
                IPersistable persistableService = workflowScope.Resolve(persistableServiceEntry.Key) as IPersistable;
                if (persistableService == null)
                {
                    throw new InvalidOperationException($"{persistableServiceEntry.Value} was registered as a persistable service but did not implement {typeof(IPersistable)}");
                }
                else
                {
                    variantProperties.Add(persistableService.GetPersistablePropertyName());
                    workflowApplication.Extensions.Add(persistableService);
                }
            }

            instanceStore.Promote("additionalProperty", variantProperties, null);
            instanceStore.InstanceCompletionAction = InstanceCompletionAction.DeleteAll;
            var instanceHandle = instanceStore.CreateInstanceHandle();
            _instanceView = instanceStore.Execute(instanceHandle, new CreateWorkflowOwnerCommand(), TimeSpan.FromSeconds(5));
            instanceHandle.Free();
            instanceStore.DefaultInstanceOwner = _instanceView.InstanceOwner;
            _instanceStore = instanceStore;
            return _instanceStore;
        }
        public bool WaitForRunnableInstance(InstanceHandle instanceHandle)
        {
            var instancePersistenceEvents = _instanceStore.WaitForEvents(instanceHandle, TimeSpan.FromHours(6));
            foreach (var instancePersistenceEvent in instancePersistenceEvents)
            {
                if (instancePersistenceEvent.Equals(HasRunnableWorkflowEvent.Value))
                {
                    return true;
                }
            }
            return false;
        }
        protected void UpdateStatus(string message)
        {
            Debug.WriteLine(message);
        }
        private void ConfigureWorkflow(WorkflowApplication workflowApplication, WorkflowHandle workflowHandle)
        {
            var workflowScope = _lifetimeScopeProvider.BeginNewLifetimeScope<ILifetimeScope,ContainerBuilder>("workflow",(builder)=>{
                builder.RegisterInstance<WorkflowHandle>(workflowHandle).As<IWorkflowHandle>();
            });
            var iocService = new IOCService(workflowScope);
            workflowApplication.Extensions.Add<IIOCService>(() => { return iocService; });

            workflowApplication.InstanceStore = GetInstanceStore(workflowApplication, workflowScope);

            workflowHandle.Bind(workflowApplication, iocService);
            workflowApplication.Completed = delegate (WorkflowApplicationCompletedEventArgs e)
            {
                if (e.CompletionState == ActivityInstanceState.Faulted)
                {
                    UpdateStatus(string.Format("Workflow Terminated. Exception: {0}\r\n{1}",
                        e.TerminationException.GetType().FullName,
                        e.TerminationException.Message));
                }
                else if (e.CompletionState == ActivityInstanceState.Canceled)
                {
                    _dataService.WorkflowInstances.SetState(workflowHandle, WorkflowInstanceStates.Canceled);
                }
                else
                {
                    _dataService.WorkflowInstances.SetState(workflowHandle, WorkflowInstanceStates.Completed);
                }
                iocService.Dispose();
                workflowScope.Dispose();
                workflowHandle.Complete(e);
            };
            workflowApplication.Aborted = delegate (WorkflowApplicationAbortedEventArgs e)
            {
                var message = $"Workflow {workflowHandle} Aborted. Exception: {e.Reason.GetType().FullName}\r\n{e.Reason.Message}";
                if (e.Reason.InnerException != null && e.Reason.InnerException != e.Reason)
                {
                    message += $"\r\n{e.Reason.InnerException.Message}";
                }
                Trace.WriteLine(message);
                workflowHandle.Terminated(message);
                _dataService.WorkflowInstances.SetState(workflowHandle, WorkflowInstanceStates.Aborted);
                iocService.Dispose();
                workflowScope.Dispose();
            };

            workflowApplication.OnUnhandledException = delegate (WorkflowApplicationUnhandledExceptionEventArgs e)
            {
                var message = $"Unhandled Exception in workflow {workflowHandle}: {e.UnhandledException.GetType().FullName}\r\n{e.UnhandledException.Message}";
                if (e.UnhandledException.InnerException != null && e.UnhandledException.InnerException != e.UnhandledException)
                {
                    message += $"\r\n{e.UnhandledException.InnerException.Message}";
                }
                Trace.WriteLine(message);
                workflowHandle.Terminated(message);
                _dataService.WorkflowInstances.SetState(workflowHandle, WorkflowInstanceStates.Faulted);
                iocService.Dispose();
                workflowScope.Dispose();
                return UnhandledExceptionAction.Terminate;
            };
            workflowApplication.PersistableIdle = delegate (WorkflowApplicationIdleEventArgs e)
            {
                var bookmarks = new List<IBookmark>();
                var workflowInstance = _dataService.WorkflowInstances.Read(workflowHandle);
                foreach (var bookmark in e.Bookmarks)
                {
                    _dataService.Bookmarks.Create(workflowInstance, bookmark.BookmarkName, bookmark.OwnerDisplayName);
                }
                _dataService.WorkflowInstances.SetState(workflowInstance, WorkflowInstanceStates.Idle);
                iocService.Dispose();
                workflowScope.Dispose();
                return PersistableIdleAction.Unload;
            };
        }
        public Activity LoadWorkflow(string workflowName)
        {
            var workflowPath = _workflowRootDirectory + workflowName + WorkflowFileExtension;
            ActivityXamlServicesSettings settings = new ActivityXamlServicesSettings
            {
                CompileExpressions = true
            };

            using (var streamReader = File.OpenText(workflowPath))
            {
                return ActivityXamlServices.Load(new XamlXmlReader(streamReader), settings);
            }
        }
        public IWorkflowHandle NewWorkflow(string packageName, string workflowName, string externalId, IDictionary<string, object> inputs)
        {
            var workflow = _dataService.Workflows.Read(packageName, workflowName);
            if (workflow == null)
            {
                throw new WorkflowNotFoundException(packageName, workflowName);
            }
            var workflowDefinition = LoadWorkflow(workflowName);
            if (workflowDefinition == null)
            {
                throw new WorkflowNotFoundException(packageName, workflowName);
            }
            var workflowIdentity = workflow.GetWorkflowIdentity();
            var workflowHandle = new WorkflowHandle(workflowIdentity, externalId);
            WorkflowApplication workflowApplication = new WorkflowApplication(workflowDefinition, inputs, workflowIdentity);
            ConfigureWorkflow(workflowApplication, workflowHandle);
            _dataService.WorkflowInstances.Create(workflowHandle, WorkflowInstanceStates.Runnable);
            return workflowHandle;
        }

        private IWorkflowHandle FromInstance(IWorkflowInstance workflowInstance)
        {
            if (workflowInstance == null)
            {
                return null;
            }

            var workflowDefinition = LoadWorkflow(workflowInstance.Workflow.Name);
            var workflowIdentity = workflowInstance.Workflow.GetWorkflowIdentity();
            var workflowHandle = new WorkflowHandle(workflowIdentity, workflowInstance.ExternalId);
            WorkflowApplication workflowApplication = new WorkflowApplication(workflowDefinition, workflowIdentity);
            ConfigureWorkflow(workflowApplication, workflowHandle);
            workflowApplication.Load(workflowInstance.InstanceId);
            workflowHandle.BookMarkResumed += (sender, args) =>
            {
                var bookmark = _dataService.Bookmarks.Read(workflowInstance.InstanceId, args.BookmarkName, BookmarkStates.Waiting);
                if (bookmark == null)
                {
                    Trace.WriteLine($"Bookmark {args.BookmarkName} caused {workflowHandle} to resume by bookmark was not found in db");
                }
                else
                {
                    _dataService.Bookmarks.SetState(bookmark, BookmarkStates.Completed);
                    _dataService.WorkflowInstances.SetState(workflowInstance, WorkflowInstanceStates.Runnable);
                }
            };
            return workflowHandle;
        }
        public IWorkflowHandle LookupWorkflow(string externalId, WorkflowInstanceStates state = WorkflowInstanceStates.Undefined, string bookMarkName = null)
        {
            return FromInstance(_dataService.WorkflowInstances.Read(externalId, state, bookMarkName));
        }
        public IWorkflowHandle LookupWorkflow(Guid instanceId)
        {
            return FromInstance(_dataService.WorkflowInstances.ReadByInstanceId(instanceId));
        }
    }
}
