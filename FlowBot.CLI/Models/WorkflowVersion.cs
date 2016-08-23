using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI.Models
{
    public class WorkflowVersion : IWorkflowVersion
    {
        public WorkflowVersion()
        {

        }
        public WorkflowVersion(string version)
        {
            if ( version == null)
            {
                throw new ArgumentNullException("version");
            }
            string[] components = version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if ( components.Length == 4)
            {
                try
                {
                    Major = int.Parse(components[0]);
                    Minor = int.Parse(components[1]);
                    Build = int.Parse(components[2]);
                    Revision = int.Parse(components[3]);
                }
                catch
                {
                    throw new InvalidCastException($"Not a valid version: {version}");
                }
            }
            else
            {
                throw new InvalidCastException($"Not a valid version: {version}");
            }
        }
        public int Build { get; set; }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Revision { get; set; }
    }
}
