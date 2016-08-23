using FlowBot.CLI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI.Interfaces
{
    public interface IFunctionContext
    {
        Options Options { get; }
        ILog Log { get; }
        RestSharp.RestClient Client { get; }
        TextReader Input { get; }
        TextWriter Output { get; }
    }
}
