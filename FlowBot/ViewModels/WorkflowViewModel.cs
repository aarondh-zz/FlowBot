using FlowBot.Common.Interfaces.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowBot.ViewModels
{
    public class WorkflowViewModel : IWorkflow
    {
        public WorkflowViewModel()
        {

        }
        public WorkflowViewModel(IWorkflow workflow)
        {
            this.Body = workflow.Body;
            this.Build = workflow.Build;
            this.CreateDate = workflow.CreateDate;
            this.Id = workflow.Id;
            this.Major = workflow.Major;
            this.Minor = workflow.Minor;
            this.Name = workflow.Name;
            this.Package = workflow.Package;
            this.Revision = workflow.Revision;

        }
        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("build")]
        public int Build { get; set; }


        public DateTime CreateDate { get; set; }


        public Guid Id { get; set; }


        [JsonProperty("major")]
        public int Major { get; set; }


        [JsonProperty("minor")]
        public int Minor { get; set; }


        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("package")]
        public string Package { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }

    }
}