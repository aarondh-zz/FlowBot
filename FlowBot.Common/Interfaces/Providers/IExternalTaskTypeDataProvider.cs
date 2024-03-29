﻿using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface IExternalTaskTypeDataProvider : IDataProvider<IExternalTaskType>
    {
        IExternalTaskType Read(string externalTaskTypeName);
    }
}
