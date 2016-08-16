using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using FlowBot.Common.Interfaces;
using FlowBot.Common.Models;
using FlowBot.Common.Interfaces.Services;

namespace FlowBotActivityLibrary
{

    public sealed class SpawnChildWorkflow : NativeActivity
    {
        private const string WorkflowBookmarkSuffix = "-child-flow-done";
        public InArgument<string> PackageName { get; set; }
        public InArgument<string> WorkflowName { get; set; }
        public InArgument<string> ExternalId { get; set; }
        public InArgument<Dictionary<string, object>> Inputs { get; set; }
        public OutArgument<IDictionary<string, object>> Outputs { get; set; }
        public OutArgument<Exception> TerminationException { get; set; }
        public OutArgument<bool> Canceled { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            var iocService = context.GetExtension<IIOCService>();
            var workflowService = iocService.Resolve<IWorkflowService>();
            var packageName = context.GetValue<string>(this.PackageName);
            var workflowName = context.GetValue<string>(this.WorkflowName);
            var externalId = context.GetValue<string>(this.WorkflowName);
            var inputs = context.GetValue<Dictionary<string, object>>(this.Inputs);
            var workflowHandle = workflowService.NewWorkflow(packageName, workflowName, externalId, inputs);
            var bookmarkName = workflowName + WorkflowBookmarkSuffix;
            var workflowInstanceId = context.WorkflowInstanceId;
            workflowHandle.Completed += (sender, e) =>
            {
                var thisWorkflow = workflowService.LookupWorkflow(workflowInstanceId);
                thisWorkflow.Resume<WorkflowCompletedEventArgs>(bookmarkName, e);
            };
            context.CreateBookmark(bookmarkName,  new BookmarkCallback(OnResumeBookmark));
        }

        protected override bool CanInduceIdle
        {
            get { return true; }
        }

        public void OnResumeBookmark(NativeActivityContext context, Bookmark bookmark, object args)
        {
            var workflowCompletedEventArgs = args as WorkflowCompletedEventArgs;
            var outputs = workflowCompletedEventArgs.Outputs;
            if (outputs == null)
            {
                outputs = new Dictionary<string, object>();
            }
            context.SetValue<IDictionary<string, object>>(this.Outputs, outputs);
            context.SetValue<bool>(this.Canceled, workflowCompletedEventArgs.Canceled);
            context.SetValue<Exception>(this.TerminationException, workflowCompletedEventArgs.TerminationException);
        }
    }
}
