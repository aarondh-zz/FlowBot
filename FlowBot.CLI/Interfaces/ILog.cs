using FlowBot.CLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI.Interfaces
{
    public interface ILog
    {
        void Write(LogLevels level, object value);
    }
}
