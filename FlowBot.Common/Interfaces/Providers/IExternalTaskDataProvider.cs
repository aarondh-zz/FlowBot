using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface IExternalTaskDataProvider : IDataProvider<IExternalTask>
    {
        IExternalTask Create(string externalTaskTypeName, string externalId, string userGroupName, object inputData);
    }
}
