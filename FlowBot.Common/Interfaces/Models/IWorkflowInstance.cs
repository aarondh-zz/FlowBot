using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IWorkflowInstance : IRecord
    {
        Guid InstanceId { get; }
        string ExternalId { get; }
        DateTime CreateDate { get; }
        Nullable<DateTime> CompletionDate { get; }
        IConversation Conversation { get; }
        IWorkflow Workflow { get; }
    }
}
