using Autofac;
using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Providers;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Data;
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

namespace FlowBot.Services
{
    public class WorkflowService : IWorkflowService
    {
        private static string workflowInstanceStoreConnectionString = ConfigurationManager.ConnectionStrings["WorkflowInstanceStore"].ConnectionString;
        private ILifetimeScopeProvider _lifetimeScopeProvider;
        private IDataService _dataService;
        private InstanceStore _instanceStore = null;
        private InstanceView _instanceView = null;
        private string _workflowRootDirectory;
        private const string WorkflowFileExtension = ".xaml";
        public WorkflowService(ILifetimeScopeProvider lifetimeScopeProvider, IDataService dataService)
        {
            _lifetimeScopeProvider = lifetimeScopeProvider;
            _dataService = dataService;
        }
        public void SetWorkflowRootDirectory( string workflowRootDirectory )
        {
            _workflowRootDirectory = workflowRootDirectory;
        }
        protected InstanceStore GetInstanceStore()
        {
            if ( _instanceStore == null)
            {
                SqlWorkflowInstanceStore instanceStore = new SqlWorkflowInstanceStore(workflowInstanceStoreConnectionString);
                instanceStore.InstanceCompletionAction = InstanceCompletionAction.DeleteAll;
                var instanceHandle = instanceStore.CreateInstanceHandle();
                _instanceView = instanceStore.Execute(instanceHandle, new CreateWorkflowOwnerCommand(), TimeSpan.FromSeconds(5));
                instanceHandle.Free();
                instanceStore.DefaultInstanceOwner = _instanceView.InstanceOwner;
                _instanceStore = instanceStore;
            }
            return _instanceStore;
        }
        protected void UpdateStatus(string message)
        {
            Debug.WriteLine(message);
        }
        private void ConfigureWorkflow( WorkflowApplication workflowApplication, WorkflowHandle workflowHandle)
        {
            var workflowScope = _lifetimeScopeProvider.BeginNewLifetimeScope<ILifetimeScope>("workflow");
            var iocService = new IOCService(workflowScope);
            workflowApplication.Extensions.Add<IIOCService>(() => { return iocService; });

            workflowApplication.InstanceStore = GetInstanceStore();

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
                    workflowHandle.Terminated("Canceled");
                }
                else
                {
                    workflowHandle.Completed(e);
                }
                iocService.Dispose();
                workflowScope.Dispose();
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
                iocService.Dispose();
                workflowScope.Dispose();
            };

            workflowApplication.OnUnhandledException = delegate (WorkflowApplicationUnhandledExceptionEventArgs e)
            {
                var message = $"Unhandled Exception in workflow {workflowHandle}: {e.UnhandledException.GetType().FullName}\r\n{e.UnhandledException.Message}";
                if ( e.UnhandledException.InnerException != null && e.UnhandledException.InnerException != e.UnhandledException)
                {
                    message += $"\r\n{e.UnhandledException.InnerException.Message}";
                }
                Trace.WriteLine(message);
               workflowHandle.Terminated(message);
                iocService.Dispose();
                workflowScope.Dispose();
                return UnhandledExceptionAction.Terminate;
            };
            workflowApplication.PersistableIdle = delegate (WorkflowApplicationIdleEventArgs e)
            {
                var identity = workflowHandle.Identity;
                var workflow = _dataService.Workflows.Read(identity.Package, identity.Name, identity.Version.Major, identity.Version.Minor, identity.Version.Build, identity.Version.Revision);
                var bookmarks = new List<IBookmark>();
                var workflowInstance = new WorkflowInstance()
                {
                    InstanceId = e.InstanceId,
                    ExternalId = workflowHandle.ExternalId,
                    Workflow = (Workflow)workflow
                };
                workflowInstance = (WorkflowInstance)_dataService.WorkflowInstances.Create(workflowInstance);
                foreach (var wfBookmark in e.Bookmarks)
                {
                    var bookmark = new Data.Bookmark()
                    {
                        Name = wfBookmark.BookmarkName,
                        OwnerDisplayName = wfBookmark.OwnerDisplayName,
                        WorkflowInstance = workflowInstance
                    };
                    _dataService.Bookmarks.Create(bookmark);
                }
                // Send the current WriteLine outputs to the status window.
                var writers = e.GetInstanceExtensions<StringWriter>();
                foreach (var writer in writers)
                {
                    UpdateStatus(writer.ToString());
                }
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
        public IWorkflowHandle NewWorkflow(string package, string workflowName, string externalId, IDictionary<string, object> inputs)
        {
            var workflow = _dataService.Workflows.Read(package, workflowName);
            var workflowDefinition = LoadWorkflow(workflowName);
            var workflowIdentity = workflow.GetWorkflowIdentity();
            var workflowHandle = new WorkflowHandle(workflowIdentity, externalId);
            workflowHandle.BookMarkResumed += (sender,args)=>
            {
                var bookmark = (Data.Bookmark)_dataService.Bookmarks.Read(externalId, args.BookmarkName);
                if ( bookmark != null)
                {
                    bookmark.CompletionDate = DateTime.UtcNow;
                    _dataService.Bookmarks.Update(bookmark);
                }
            };
            WorkflowApplication workflowApplication = new WorkflowApplication(workflowDefinition, inputs, workflowIdentity);
            ConfigureWorkflow(workflowApplication, workflowHandle);
            return workflowHandle;
        }

        private void WorkflowHandle_BookMarkResumed(object sender, Models.BookMarkResumedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public IWorkflowHandle LookupWorkflow(string externalId)
        {
            var workflowInstance = _dataService.WorkflowInstances.Read(externalId);
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
            return workflowHandle;
        }
    }
}
