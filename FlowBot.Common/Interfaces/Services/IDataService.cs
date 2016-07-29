﻿using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Services
{
    public interface IDataService
    {
        IWorkflow GetWorkflow(string name, string version = null);
    }
}
