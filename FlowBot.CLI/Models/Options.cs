using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI.Models
{
    public class Options
    {
        [Option('s', "server", Required = true, HelpText = "FlowBot server url")]
        public string ServerUrl { get; set; }
        [Option('i', "input", Required = false, HelpText = "The path of the input file")]
        public string Input { get; set; }
        [Option('o', "output", Required = false, HelpText = "The path of the output file")]
        public string Output { get; set; }
        [Option('n', "name", DefaultValue = "*", Required = false, HelpText = "The name of the workflow")]
        public string Package { get; set; }
        [Option('p', "package", DefaultValue = "*", Required = false, HelpText = "The package the workflow belongs to")]
        public string Name { get; set; }
        [Option('v', "version", DefaultValue = "1.0.0.0", HelpText = "The version of the workflow (defaults to 1.0.0.0)")]
        public string Version { get; set; }
        [Option("logLevel", DefaultValue = LogLevels.Info, Required = false, HelpText = "set to Error, Warning, Info, or Verbose (defaults to Warning)")]
        public LogLevels LogLevel { get; set; }
        [Option('a', "action", DefaultValue = Actions.Read, HelpText = "Action to be performed - Create, Read, Update, Delete, or List")]
        public Actions Action { get; set; }
        [Option('f', "function", DefaultValue = Functions.Help, HelpText = "Read the workflow")]
        public Functions Function { get; set; }
        [Option('1', "skip", Required = false, HelpText = "Skip x items")]
        public int? Skip { get; set; }
        [Option('2', "take", Required = false, HelpText = "Take x items")]
        public int? Take { get; set; }
        [Option("id", Required = false, HelpText = "Id to be read or deleted")]
        public string Id { get; set; }
        [Option("pause", Required = false, HelpText = "Pause and wait for a carriage return")]
        public bool Pause { get; set; }
    }
}
