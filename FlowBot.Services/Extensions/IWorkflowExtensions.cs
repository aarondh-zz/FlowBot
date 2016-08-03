using FlowBot.Common.Interfaces.Models;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Services.Extensions
{
    public static class IWorkflowExtensions
    {
        public static WorkflowIdentity GetWorkflowIdentity(this IWorkflow workflow)
        {
            return new WorkflowIdentity(workflow.Name, new Version(workflow.Major, workflow.Minor, workflow.Build, workflow.Revision), workflow.Package);
        }
    }
}
