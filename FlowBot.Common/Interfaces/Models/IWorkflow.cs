using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IWorkflow
    {
        string Name { get; }
        string Version { get; }
        string Body { get; }
    }
}
