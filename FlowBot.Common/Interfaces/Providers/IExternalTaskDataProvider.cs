using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface IExternalTaskDataProvider : IDataProvider<IExternalTask>
    {
        IExternalTask Create(IWorkflowInstance workflowInstance, string externalTaskTypeName, string externalId, string userGroupName, object inputData, string bookmarkName);
        IOrderedQueryable<IExternalTask> List(string groupName = null, Guid? workerId = null, ExternalTaskStates? state = null, OrderBy orderBy = OrderBy.Unordered);
        void SetState(IExternalTask task, Guid workerId, ExternalTaskStates state, string outputData);

    }
}
