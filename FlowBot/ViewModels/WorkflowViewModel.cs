using FlowBot.Common.Interfaces.Models;
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
        public string Body { get; set; }

        public int Build { get; set; }


        public DateTime CreateDate { get; set; }


        public Guid Id { get; set; }


        public int Major { get; set; }


        public int Minor { get; set; }


        public string Name { get; set; }

        public string Package { get; set; }

        public int Revision { get; set; }

    }
}