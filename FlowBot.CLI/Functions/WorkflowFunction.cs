using FlowBot.CLI.Interfaces;
using FlowBot.CLI.Models;
using FlowBot.CLI.Utilities;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI.Functions
{
    internal class WorkflowFunction : IFunction
    {
        public void Create(IFunctionContext context)
        {
            context.Log.Write(LogLevels.Info,$"writing {context.Options.Package}:{context.Options.Name}:{context.Options.Version} to {context.Options.ServerUrl}");
            var workflowRequest = new RestRequest("api/workflow", Method.POST);
            var workflow = new Workflow();
            if (context.Options.Package == "*")
            {
                throw new InvalidOperationException("read requires an package to be specified");
            }
            if (context.Options.Name == "*")
            {
                throw new InvalidOperationException("read requires an name to be specified");
            }
            if (context.Options.Input == null)
            {
                throw new InvalidOperationException("read requires an workflow file path to be specified as --input");
            }
            else
            {
                context.Log.Write(LogLevels.Info, $"workflow source read from {context.Options.Input}");
            }
            workflow.Package = context.Options.Package;
            workflow.Name = context.Options.Name;
            var workflowVersion = new WorkflowVersion(context.Options.Version);
            workflow.Major = workflowVersion.Major;
            workflow.Minor = workflowVersion.Minor;
            workflow.Build = workflowVersion.Build;
            workflow.Revision = workflowVersion.Revision;
            workflow.Body = context.Input.ReadToEnd();
            workflowRequest.AddJsonBody(workflow);
            var response = context.Client.Execute(workflowRequest);
            Console.WriteLine(response.Content);
        }

        public void Delete(IFunctionContext context)
        {
            if (context.Options.Id == null)
            {
                throw new InvalidOperationException("delete requires an --id to be specified");
            }
            context.Log.Write(LogLevels.Info,$"deleting {context.Options.Package}:{context.Options.Name}:{context.Options.Version} from {context.Options.ServerUrl}");
            if (context.Options.Id == null)
            {
                throw new InvalidOperationException("read requires an id to be specified");
            }
            var workflowRequest = new RestRequest("api/workflow/{id}", Method.DELETE);
            workflowRequest.AddParameter("id", context.Options.Id);
            var response = context.Client.Execute(workflowRequest);
            response.ReportComplete(context.Log, "workflow create complete");
        }

        public void List(IFunctionContext context)
        {
            context.Log.Write(LogLevels.Info, $"listing workflows {context.Options.Package}:{context.Options.Name} from {context.Options.ServerUrl}");
            var workflowRequest = new RestRequest("api/workflow");
            if (context.Options.Package != "*")
            {
                workflowRequest.AddParameter("package", context.Options.Package);
            }
            if (context.Options.Name != "*")
            {
                workflowRequest.AddParameter("name", context.Options.Name);
            }
            if (context.Options.Skip.HasValue)
            {
                workflowRequest.AddParameter("skip", context.Options.Skip.Value);
            }
            if (context.Options.Take.HasValue)
            {
                workflowRequest.AddParameter("take", context.Options.Take.Value);
            }
            var response = context.Client.Execute<List<Workflow>>(workflowRequest);
            response.ReportComplete(context.Log, "workflow list complete");
            foreach (var workflow in response.Data)
            {
                int contentSize = workflow.Body == null ? 0 : workflow.Body.Length;
                context.Log.Write(LogLevels.Info, $"{workflow.Package}:{workflow.Name}:{workflow.Major}.{workflow.Minor}.{workflow.Build}.{workflow.Revision} {workflow.Id} {workflow.CreateDate} ({contentSize} characters)");
            }
            context.Log.Write(LogLevels.Info, $"{response.Data.Count} workflows read");
        }

        public void Read(IFunctionContext context)
        {
            if (context.Options.Id == null)
            {
                throw new InvalidOperationException("read requires an --id to be specified");
            }
            var workflowRequest = new RestRequest("api/workflow/{id}");
            workflowRequest.AddParameter("id", context.Options.Id);
            context.Log.Write(LogLevels.Info, $"reading workflow {context.Options.Id} from {context.Options.ServerUrl}");
            var response = context.Client.Execute<Workflow>(workflowRequest);
            response.ReportComplete(context.Log, "workflow read complete");
            context.Output.Write(response.Data.Body);
            if (context.Options.Output != null)
            {
                context.Log.Write(LogLevels.Info, $"workflow source written to {context.Options.Output}");
            }
        }

        public void Update(IFunctionContext context)
        {
            if (context.Options.Id == null)
            {
                throw new InvalidOperationException("read requires an id to be specified");
            }
            if (context.Options.Input == null)
            {
                throw new InvalidOperationException("patch requires an workflow file path to be specified for --input");
            }
            else
            {
                context.Log.Write(LogLevels.Info, $"workflow source read from {context.Options.Input}");
            }
            var workflowRequest = new RestRequest("api/workflow/{id}", Method.PATCH);
            workflowRequest.AddParameter("id",context.Options.Id, ParameterType.UrlSegment);
            workflowRequest.JsonSerializer = new FlowBot.CLI.JsonSerializer();
            var body = context.Input.ReadToEnd();
            workflowRequest.AddJsonBody(new Patch[] { new Patch(PatchOperators.Replace, "/body", body) });
            context.Log.Write(LogLevels.Info, $"updating workflow {context.Options.Id} from {context.Options.ServerUrl}");
            var response = context.Client.Execute<Workflow>(workflowRequest);
            response.ReportComplete(context.Log, "workflow update complete");
        }
    }
}
