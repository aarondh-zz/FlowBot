using Autofac;
using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Models;
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
        private static string workflowInstanceStoreConnectionString = null;
        private static InstanceStore s_instanceStore = null;

        public static IWorkflowServiceHost Instance = new WorkflowServiceHost();

        private WorkflowServiceHost()
        {

        }
        public IWorkflowHandle LookupWorkflow(string externalId)
        {
            return null;
        }
        protected void UpdateStatus(string message)
        {
            Debug.WriteLine(message);
        }
        private void ConfigureWorkflow(ILifetimeScope lifetimeScope, WorkflowApplication workflowApplication, WorkflowHandle workflowHandle)
        {
            if ( s_instanceStore == null)
            {
                s_instanceStore = new SqlWorkflowInstanceStore(workflowInstanceStoreConnectionString);
            }
            workflowApplication.Extensions.Add<IIOCService>(() => { return lifetimeScope.ResolveOptional<IIOCService>( new NamedParameter("lifetimeScope",lifetimeScope) ); });

            workflowApplication.Extensions.Add<IConnectorService>(() => { return lifetimeScope.ResolveOptional<IConnectorService>(); });

            workflowApplication.Extensions.Add<ILuisService>(() => { return lifetimeScope.ResolveOptional<ILuisService>(); });

            workflowApplication.Extensions.Add<IDataService>(() => { return lifetimeScope.ResolveOptional<IDataService>(); });

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
            };
            workflowApplication.Aborted = delegate (WorkflowApplicationAbortedEventArgs e)
            {
                workflowHandle.Terminated($"Workflow Aborted. Exception: {e.Reason.GetType().FullName}\r\n{e.Reason.Message}");
            };

            workflowApplication.OnUnhandledException = delegate (WorkflowApplicationUnhandledExceptionEventArgs e)
            {
                workflowHandle.Terminated($"Unhandled Exception: {e.UnhandledException.GetType().FullName}\r\n{e.UnhandledException.Message}");
                return UnhandledExceptionAction.Terminate;
            };
            workflowApplication.PersistableIdle = delegate (WorkflowApplicationIdleEventArgs e)
            {
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
        public IWorkflowHandle RunNewWorkflow(object lifetimeScope, string workflowPath, IDictionary<string, object> inputs)
        {
            var workflowDefinition = LoadWorkflow(workflowPath);
            var workflowIdentity = new WorkflowIdentity(System.IO.Path.GetFileNameWithoutExtension(workflowPath), new Version(1, 0, 0, 0), "Flowbot");
            var workflowHandle = new WorkflowHandle(workflowIdentity, null, null);
            WorkflowApplication workflowApplication = new WorkflowApplication(workflowDefinition, inputs, workflowIdentity);
            ConfigureWorkflow(lifetimeScope as ILifetimeScope, workflowApplication, workflowHandle);
            workflowApplication.Run();
            return workflowHandle;
        }
    }
}
