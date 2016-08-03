using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Data
{
    public partial class WorkflowInstance : IWorkflowInstance
    {

        IWorkflow IWorkflowInstance.Workflow
        {
            get
            {
                return this.Workflow;
            }
        }
        IConversation IWorkflowInstance.Conversation
        {
            get
            {
                return this.Conversation;
            }
        }
    }
}
