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
        public Guid InstanceId
        {
            get
            {
                return _application.Id;
            }
        }
        private class WorkflowIdentityWrapper : IWorkflowIdentity
        {
            private WorkflowIdentity _identity;
            public int Build { get { return _identity.Version.Build; } }
            public int Major { get { return _identity.Version.Major; } }
            public int Minor { get { return _identity.Version.Minor; } }
            public string Name { get { return _identity.Name; } }
            public string Package { get { return _identity.Package; } }
            public int Revision { get { return _identity.Version.Revision; } }
            public WorkflowIdentityWrapper( WorkflowIdentity identity)
            {
                _identity = identity;
            }

            public override string ToString()
            {
                return _identity.ToString();
            }
        }
        public IWorkflowIdentity Identity { get; private set; }

        private WorkflowApplication _application;
        private IIOCService _iocService;
        public WorkflowHandle(WorkflowIdentity identity, string externalId)
        {
            this.Identity = new WorkflowIdentityWrapper(identity);
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
