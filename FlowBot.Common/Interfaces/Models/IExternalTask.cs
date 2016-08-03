using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IExternalTask
    {
        IExternalTaskType ExternalTaskType { get; }
        IUser Worker { get; }
        IUserGroup UserGroup { get; }
        Nullable<DateTime> ClaimDate { get; }
        Nullable<DateTime> CompletionDate { get; }
        string InputData { get; }
        string OutputData { get; }
    }
}
