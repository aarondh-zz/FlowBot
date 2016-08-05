using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Data
{
    public partial class ExternalTask : IExternalTask
    {
        IExternalTaskType IExternalTask.ExternalTaskType
        {
            get
            {
                return this.ExternalTaskType;
            }
        }

        IUserGroup IExternalTask.UserGroup
        {
            get
            {
                return this.UserGroup;
            }
        }

        IUser IExternalTask.Worker
        {
            get
            {
                return this.Worker;
            }
        }
        IWorkflowInstance IExternalTask.WorkflowInstance
        {
            get
            {
                return this.WorkflowInstance;
            }
        }
    }
}
