using Autofac;
using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Providers;
using FlowBot.Common.Interfaces.Services;
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
    public class WorkflowServiceHost : IWorkflowServiceHost
    {
        private static string workflowInstanceStoreConnectionString = ConfigurationManager.AppSettings["WorkflowInstanceStore"];
        private ILifetimeScopeProvider _lifetimeScopeProvider;
        private IDataService _dataService;
        private InstanceStore _instanceStore = null;
        public WorkflowServiceHost(ILifetimeScopeProvider lifetimeScopeProvider, IDataService dataService)
        {
            _lifetimeScopeProvider = lifetimeScopeProvider;
            _instanceStore = new SqlWorkflowInstanceStore(workflowInstanceStoreConnectionString);
            _dataService = dataService;
        }
        public IWorkflowHandle LookupWorkflow(string externalId)
        {
            return null;
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

            workflowApplication.InstanceStore = _instanceStore;

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
                iocService.Dispose();
                workflowScope.Dispose();
                workflowHandle.Terminated($"Workflow Aborted. Exception: {e.Reason.GetType().FullName}\r\n{e.Reason.Message}");
            };

            workflowApplication.OnUnhandledException = delegate (WorkflowApplicationUnhandledExceptionEventArgs e)
            {
                iocService.Dispose();
                workflowScope.Dispose();
                workflowHandle.Terminated($"Unhandled Exception: {e.UnhandledException.GetType().FullName}\r\n{e.UnhandledException.Message}");
                return UnhandledExceptionAction.Terminate;
            };
            workflowApplication.PersistableIdle = delegate (WorkflowApplicationIdleEventArgs e)
            {
                iocService.Dispose();
                workflowScope.Dispose();
                // Send the current WriteLine outputs to the status window.
                var writers = e.GetInstanceExtensions<StringWriter>();
                foreach (var writer in writers)
                {
                    UpdateStatus(writer.ToString());
                }
                return PersistableIdleAction.Unload;
            };
        }
        public Activity LoadWorkflow(string workflowName)
        {
            ActivityXamlServicesSettings settings = new ActivityXamlServicesSettings
            {
                CompileExpressions = true
            };

            using (var streamReader = File.OpenText(workflowName))
            {
                return ActivityXamlServices.Load(new XamlXmlReader(streamReader), settings);
            }
        }
        public IWorkflowHandle NewWorkflow(string workflowName, string externalId, IDictionary<string, object> inputs)
        {
            var workflowDefinition = LoadWorkflow(workflowName);
            var workflowIdentity = new WorkflowIdentity(System.IO.Path.GetFileNameWithoutExtension(workflowName), new Version(1, 0, 0, 0), "Flowbot");
            var workflowHandle = new WorkflowHandle(workflowIdentity);
            WorkflowApplication workflowApplication = new WorkflowApplication(workflowDefinition, inputs, workflowIdentity);
            ConfigureWorkflow(workflowApplication, workflowHandle);
            return workflowHandle;
        }
    }
}
