﻿using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface IWorkflowInstanceDataProvider : IDataProvider<IWorkflowInstance>
    {

        IWorkflowInstance Read(string externalId, WorkflowInstanceStates state = WorkflowInstanceStates.Undefined, string bookMarkName = null);
        IWorkflowInstance Read(IWorkflowHandle workflowHandle);
        IWorkflowInstance ReadByInstanceId(Guid instanceId);
        IWorkflowInstance Create(IWorkflowHandle workflowHandle, WorkflowInstanceStates state = WorkflowInstanceStates.Runnable, Nullable<DateTime> completionDate = null);
        void SetState(IWorkflowInstance workflowInstance, WorkflowInstanceStates state);
        void SetState(IWorkflowHandle workflowHandle, WorkflowInstanceStates state);
    }
}
