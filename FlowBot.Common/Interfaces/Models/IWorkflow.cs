using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IWorkflow : IRecord
    {
        string Name { get; }
        string Package { get; }
        int Major { get; }
        int Minor { get; }
        int Build { get; }
        int Revision { get; }
        string Body { get; }
    }
}
