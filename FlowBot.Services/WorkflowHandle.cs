using FlowBot.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using FlowBot.Common.Interfaces.Models;

namespace FlowBot.Services
{
    internal class WorkflowHandle : IWorkflowHandle
    {
        private Action<WorkflowApplicationCompletedEventArgs> _completed;
        private Action<string> _terminated;
        public WorkflowIdentity Identity { get; private set; }
        public WorkflowHandle(WorkflowIdentity identity, Action<WorkflowApplicationCompletedEventArgs> completed, Action<string> terminated)
        {
            this.Identity = identity;
            _completed = completed;
            _terminated = terminated;
        }

        public void Completed(WorkflowApplicationCompletedEventArgs args)
        {
            _completed?.Invoke(args);
        }
        public void Terminated(string reason)
        {
            _terminated?.Invoke(reason);
        }
    }
}
