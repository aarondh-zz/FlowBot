using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Models;
using System;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IWorkflowHandle
    {
        event EventHandler<WorkflowCompletedEventArgs> Completed;
        string ExternalId { get; }
        Guid InstanceId { get; }
        IWorkflowIdentity Identity { get; }
        IIOCService IOCService { get; }
        void Resume<T>(string bookmarkName, T bookmarkData);
        void Resume(string bookmarkName);
        void Run();
    }
}
