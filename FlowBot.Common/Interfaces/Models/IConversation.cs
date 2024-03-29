﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IConversation : IRecord
    {
        string ExternalId { get; }
        IWorkflowInstance WorkflowInstance { get; }
    }
}
