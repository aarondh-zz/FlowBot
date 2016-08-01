using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Services
{
    public interface IDataService
    {
        IWorkflowDataProvider Workflows { get; }
        IUserDataProvider Users { get; }
        IConversationDataProvider Conversations { get; }
    }
}
