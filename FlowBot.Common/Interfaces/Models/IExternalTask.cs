using FlowBot.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IExternalTask : IRecord
    {
        IExternalTaskType ExternalTaskType { get; }
        IWorkflowInstance WorkflowInstance { get; }
        IUser Worker { get; }
        IUserGroup UserGroup { get; }
        ExternalTaskStates State { get;  }
        Nullable<DateTime> ClaimDate { get; }
        Nullable<DateTime> CompletionDate { get; }
        string ExternalId { get; }
        string InputData { get; }
        string OutputData { get; }
        string BookmarkName { get; }
    }
}
