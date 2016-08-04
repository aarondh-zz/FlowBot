using FlowBot.Common.Interfaces.Services;
using System;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IWorkflowHandle
    {
        string ExternalId { get; }
        Guid InstanceId { get; }
        IWorkflowIdentity Identity { get; }
        IIOCService IOCService { get; }
        void Resume<T>(string bookmarkName, T bookmarkData);
        void Resume(string bookmarkName);
        void Run();
    }
}
