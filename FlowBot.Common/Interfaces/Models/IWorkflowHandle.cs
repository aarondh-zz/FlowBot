using FlowBot.Common.Interfaces.Services;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IWorkflowHandle
    {
        WorkflowIdentity Identity { get; }
        IIOCService IOCService { get; }
        void Resume<T>(string bookmarkName, T bookmarkData);
        void Resume(string bookmarkName);
        void Run();
    }
}
