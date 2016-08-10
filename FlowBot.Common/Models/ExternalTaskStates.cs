using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Models
{
    public enum ExternalTaskStates
    {
        Undefined = 0,
        Queued = 1,
        Claimed = 2,
        Completed = 3,
        Failed = 4,
        Error = 5
    }
}
