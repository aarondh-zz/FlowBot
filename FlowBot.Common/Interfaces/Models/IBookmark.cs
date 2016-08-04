using FlowBot.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IBookmark : IRecord
    {
        string Name { get; }
        string OwnerDisplayName { get; }
        BookmarkStates State { get; }
        Nullable<DateTime> CompletionDate { get; }
        IWorkflowInstance WorkflowInstance { get; }
    }
}
