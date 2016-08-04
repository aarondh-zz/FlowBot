using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Exceptions
{
    public class WorkflowNotFoundException : Exception
    {
        public string PackageName { get; private set; }
        public string WorkflowName { get; private set; }

        public WorkflowNotFoundException( string packageName, string workflowName) : base($"Package \"{packageName}\" Workflow \"{workflowName}\" was not found")
        {
            this.PackageName = packageName;
            this.WorkflowName = workflowName;
        }
    }
}
