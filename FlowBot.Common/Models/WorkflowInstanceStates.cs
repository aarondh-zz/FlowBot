using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Models
{
    public enum WorkflowInstanceStates : int
    {
        Undefined = 0,
        Runnable = 1,
        Idle = 2,
        Faulted = 3,
        Completed = 4,
        Canceled = 5,
        Aborted = 6
    }
}
