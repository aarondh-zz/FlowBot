﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IEntity
    {
        string Text { get; }
        double? Score { get; }
    }
}
