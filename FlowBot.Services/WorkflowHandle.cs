using FlowBot.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Services;

namespace FlowBot.Services
{
    internal class WorkflowHandle : IWorkflowHandle
    {
        public WorkflowIdentity Identity { get; private set; }

        private WorkflowApplication _application;
        private IIOCService _iocService;
        public WorkflowHandle(WorkflowIdentity identity)
        {
            this.Identity = identity;
        }

        public void Bind( WorkflowApplication application, IIOCService iocService)
        {
            _application = application;
            _iocService = iocService;
        }

        public void Run()
        {
            _application.Run();
        }
        public void Resume<T>(string bookmarkName, T bookmarkData)
        {
            throw new NotImplementedException();
        }
        public void Resume(string bookmarkName)
        {
            throw new NotImplementedException();
        }

        public IIOCService IOCService
        {
            get
            {
                return _iocService;
            }
        }
        public void Completed(WorkflowApplicationCompletedEventArgs args)
        {
        }
        public void Terminated(string reason)
        {
        }
    }
}
