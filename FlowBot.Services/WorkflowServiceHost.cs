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
        private static InstanceStore s_instanceStore = null;
        private ILifetimeScopeProvider _lifetimeScopeProvider;
        public WorkflowServiceHost(ILifetimeScopeProvider lifetimeScopeProvider)
        {
            _lifetimeScopeProvider = lifetimeScopeProvider;
        }
        public IWorkflowHandle LookupWorkflow(string externalId)
        {
            return null;
        }
        protected void UpdateStatus(string message)
        {
            Debug.WriteLine(message);
        }
        private void ConfigureWorkflow( WorkflowApplication workflowApplication, WorkflowHandle workflowHandle, object connectorActivity)
        {
            var workflowScope = _lifetimeScopeProvider.BeginNewLifetimeScope<ILifetimeScope>("workflow");
            var iocService = new IOCService(workflowScope);
            if ( s_instanceStore == null)
            {
                s_instanceStore =new SqlWorkflowInstanceStore(workflowInstanceStoreConnectionString);
            }
            var connectorService = iocService.Resolve<IConnectorService>();
            connectorService.BindActivity(connectorActivity);
            workflowApplication.Extensions.Add<IIOCService>(() => { return iocService; });

            workflowApplication.InstanceStore = s_instanceStore;
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

        public Activity LoadWorkflow(string workflowPath)
        {
            ActivityXamlServicesSettings settings = new ActivityXamlServicesSettings
            {
                CompileExpressions = true
            };

            using (var streamReader = File.OpenText(workflowPath))
            {
                return ActivityXamlServices.Load(new XamlXmlReader(streamReader), settings);
            }
        }
        public IWorkflowHandle RunNewWorkflow(string workflowPath, IDictionary<string, object> inputs, object connectorActivity)
        {
            var workflowDefinition = LoadWorkflow(workflowPath);
            var workflowIdentity = new WorkflowIdentity(System.IO.Path.GetFileNameWithoutExtension(workflowPath), new Version(1, 0, 0, 0), "Flowbot");
            var workflowHandle = new WorkflowHandle(workflowIdentity, null, null);
            WorkflowApplication workflowApplication = new WorkflowApplication(workflowDefinition, inputs, workflowIdentity);
            ConfigureWorkflow(workflowApplication, workflowHandle, connectorActivity);
            workflowApplication.Run();
            return workflowHandle;
        }
    }
}
