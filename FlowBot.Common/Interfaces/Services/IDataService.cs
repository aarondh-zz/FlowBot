﻿using FlowBot.Common.Interfaces.Models;
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
        IBookmarkDataProvider Bookmarks { get; }
        IConversationDataProvider Conversations { get; }
        IExternalTaskDataProvider ExternalTasks { get; }
        IExternalTaskTypeDataProvider ExternalTaskTypes { get; }
        IMessageDataProvider Messages { get; }
        IUserDataProvider Users { get; }
        IUserGroupDataProvider UserGroups { get; }
        IWorkflowDataProvider Workflows { get; }
        IWorkflowInstanceDataProvider WorkflowInstances { get; }
    }
}
