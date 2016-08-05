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

    public sealed class ExternalTaskActivity : NativeActivity
    {
        public InArgument<string> ExternalTaskType { get; set; }
        public InArgument<string> ExternalId { get; set; }
        public InArgument<string> UserGroup { get; set; }
        public InArgument<Dictionary<string, object>> InputData { get; set; }
        public OutArgument<Dictionary<string, object>> OutputData { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            var iocService = context.GetExtension<IIOCService>();
            var dataService = iocService.Resolve<IDataService>();
            var externalTaskType = context.GetValue<string>(this.ExternalTaskType);
            var externalId = context.GetValue<string>(this.ExternalId);
            var userGroup = context.GetValue<string>(this.UserGroup);
            var inputData = context.GetValue<Dictionary<string, object>>(this.InputData);
            var workflowInstance = dataService.WorkflowInstances.ReadByInstanceId(context.WorkflowInstanceId);
            dataService.ExternalTasks.Create(workflowInstance, externalTaskType, externalId, userGroup, inputData,"task-done");
            context.CreateBookmark("task-done",  new BookmarkCallback(OnResumeBookmark));
        }
        protected override bool CanInduceIdle
        {
            get { return true; }
        }

        public void OnResumeBookmark(NativeActivityContext context, Bookmark bookmark, object outputData)
        {
            var outputDataDictionary = outputData as Dictionary<string, object>;
            if ( outputDataDictionary == null)
            {
                outputDataDictionary = new Dictionary<string, object>();
            }
            context.SetValue<Dictionary<string, object>>(this.OutputData, outputDataDictionary);
        }
    }
}
