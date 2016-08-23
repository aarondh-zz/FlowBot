using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI.Models
{
    public class Workflow : IWorkflow
    {
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
