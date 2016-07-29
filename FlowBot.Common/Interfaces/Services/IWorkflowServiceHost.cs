﻿using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Services
{
    public interface IWorkflowServiceHost
    {
        IWorkflowHandle LookupWorkflow(string externalId);
        IWorkflowHandle RunNewWorkflow(string workflowPath, IDictionary<Type, object> extensions, IDictionary<string, object> inputs);
    }
}