using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Services
{
    public interface IWorkflowService
    {
        void SetWorkflowRootDirectory(string workflowRootDirectory);
        IWorkflowHandle LookupWorkflow(string externalId);
        IWorkflowHandle LookupWorkflow(Guid instanceId);
        IWorkflowHandle NewWorkflow(string package, string workflowName, string externalId, IDictionary<string, object> inputs);
    }
}
