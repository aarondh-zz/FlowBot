using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Models
{
    public class WorkflowCompletedEventArgs : EventArgs
    {
        public IDictionary<string, object> Outputs { get; private set; }
        public bool Canceled { get; private set; }
        public Exception TerminationException { get; private set; }

        public WorkflowCompletedEventArgs(IDictionary<string, object> outputs, bool canceled, Exception terminationException)
        {
            this.Outputs = outputs;
            this.Canceled = canceled;
            this.TerminationException = terminationException;
        }
    }
}
