using FlowBot.Common.Interfaces.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowBot.ViewModels
{
    public class BookmarkParametersViewModel : IBookmarkParameters
    {
        public Guid WorkflowInstanceId { get; set; }
        public string BookmarkName { get; set; }
        public string Data { get; set; }
    }
}