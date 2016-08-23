using FlowBot.CLI.Interfaces;
using FlowBot.CLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI
{
    public class ConsoleLog : ILog
    {
        private Options _options;
        public ConsoleLog(Options options)
        {
            _options = options;
        }
        public void Write(LogLevels level, object value)
        {
            switch (level)
            {
                case LogLevels.Error:
                    if (level <= _options.LogLevel)
                    {
                        Console.Error.WriteLine("Error: " + value);
                    }
                    break;
                case LogLevels.Warning:
                    if (level <= _options.LogLevel)
                    {
                        Console.Error.WriteLine("Warning: " + value);
                    }
                    break;
                case LogLevels.Info:
                    if (level <= _options.LogLevel)
                    {
                        Console.Out.WriteLine("Info: " + value);
                    }
                    break;
                case LogLevels.Verbose:
                    if (level <= _options.LogLevel)
                    {
                        Console.Out.WriteLine("Verbose: " + value);
                    }
                    break;
            }
        }
    }
}
