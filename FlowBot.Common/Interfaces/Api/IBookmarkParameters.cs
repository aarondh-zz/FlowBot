using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Api
{
    public interface IBookmarkParameters
    {
        Guid WorkflowInstanceId { get; set; }
        string BookmarkName { get; set; }
        string Data { get; set; }
    }
}
