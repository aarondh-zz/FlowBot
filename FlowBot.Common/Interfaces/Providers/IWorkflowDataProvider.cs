using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface IWorkflowDataProvider : IDataProvider<IWorkflow>
    {
        IWorkflow Read(string package, string name, Nullable<int> major = null, Nullable<int> minor = null, Nullable<int> build = null, Nullable<int> revision = null);
    }
}
