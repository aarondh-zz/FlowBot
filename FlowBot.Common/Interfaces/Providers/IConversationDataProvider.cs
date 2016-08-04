using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface IConversationDataProvider : IDataProvider<IConversation>
    {
        IConversation Create(IWorkflowHandle workflowHandle, string externalId);
        IConversation Read(IWorkflowHandle workflowHandle, string externalId);
        IConversation ReadOrCreate(IWorkflowHandle workflowHandle, string externalId);
    }
}
