using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IWorkflowVersion
    {
        int Major { get; }
        int Minor { get; }
        int Build { get; }
        int Revision { get; }
    }
}
