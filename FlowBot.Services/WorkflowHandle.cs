using FlowBot.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Services.Models;

namespace FlowBot.Services
{
    internal class WorkflowHandle : IWorkflowHandle
    {
        public event EventHandler<BookMarkResumedEventArgs> BookMarkResumed;
        public string ExternalId { get; private set; }
        public WorkflowIdentity Identity { get; private set; }

        private WorkflowApplication _application;
        private IIOCService _iocService;
        public WorkflowHandle(WorkflowIdentity identity, string externalId)
        {
            this.Identity = identity;
            this.ExternalId = externalId;
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
            var result = _application.ResumeBookmark(bookmarkName, bookmarkData);
            switch(result)
            {
                case BookmarkResumptionResult.NotFound:
                    throw new Exception($"Bookmark {bookmarkName} was not found in workflow {this}");
                case BookmarkResumptionResult.NotReady:
                    throw new Exception($"Workflow {this} was not ready to Resume Bookmark {bookmarkName}");
                case BookmarkResumptionResult.Success:
                    BookMarkResumed?.Invoke(this, new BookMarkResumedEventArgs(bookmarkName));
                    break;
            }
        }
        public void Resume(string bookmarkName)
        {
            _application.ResumeBookmark(bookmarkName, null);
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

        public override string ToString()
        {
            return this.Identity.ToString() + "; External id: " + this.ExternalId;
        }
    }
}
