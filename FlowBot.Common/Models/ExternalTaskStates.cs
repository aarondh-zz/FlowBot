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
        Open = 1,
        Completed = 2,
        Failed = 3,
        Error = 4
    }
}
