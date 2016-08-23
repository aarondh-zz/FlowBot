using FlowBot.CLI.Functions;
using FlowBot.CLI.Interfaces;
using FlowBot.CLI.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var title = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
            var version = ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyFileVersionAttribute), false)).Version;
            version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var copyright = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright;
            var options = new Options();
            Console.WriteLine($"{title} {version}");
            Console.WriteLine(copyright);
            var log = new ConsoleLog(new Models.Options());
            try
            {
                var results = CommandLine.Parser.Default.ParseArguments<Options>(args);
                options = results.Value;
                log = new ConsoleLog(options);
                if (results.Errors.Count() == 0)
                {
                    var flowBotServer = new RestClient(options.ServerUrl);
                    using (var context = new FunctionContext(options, flowBotServer, log, () =>
                      {
                          if (options.Input == null)
                          {
                              return Console.In;
                          }
                          else
                          {
                              return File.OpenText(options.Input);
                          }
                      }, () =>
                      {
                          if (options.Output == null)
                          {
                              return Console.Out;
                          }
                          else
                          {
                              return File.CreateText(options.Output);
                          }

                      }))
                    {
                        IFunction function;
                        switch (options.Function)
                        {
                            case Models.Functions.Workflow:
                                function = new WorkflowFunction();
                                break;
                            case Models.Functions.User:
                                throw new NotImplementedException("--function User is not implemented");
                            case Models.Functions.Bookmark:
                                throw new NotImplementedException("--function Bookmark is not implemented");
                            default:
                                throw new NotSupportedException($"--function {options.Function} is not supported");
                        }
                        switch (options.Action)
                        {
                            case Actions.Create:
                                function.Create(context);
                                break;
                            case Actions.Read:
                                function.Read(context);
                                break;
                            case Actions.Update:
                                function.Update(context);
                                break;
                            case Actions.Delete:
                                function.Delete(context);
                                break;
                            case Actions.List:
                                function.List(context);
                                break;
                            default:
                                throw new NotSupportedException($"--action {options.Action} is not supported");
                        }
                    }
                }
                PauseIfRequired(options);
                return 0;
            }
            catch (InvalidOperationException e)
            {
                log.Write(LogLevels.Error,e.Message);
            }
            catch (NotSupportedException e)
            {
                log.Write(LogLevels.Error, e.Message);
            }
            catch (NotImplementedException e)
            {
                log.Write(LogLevels.Error, e.Message);
            }
            catch (Exception e)
            {
                log.Write(LogLevels.Error, "Unhandled exception: " + e.Message);
                log.Write(LogLevels.Verbose, e.ToString());
            }
            PauseIfRequired(options);
            return -3;
        }
        public static void PauseIfRequired(Options options)
        {
            if (options.Pause)
            {
                Console.Write("Press carriage return to continue>");
                Console.ReadLine();
            }
        }
    }
}
