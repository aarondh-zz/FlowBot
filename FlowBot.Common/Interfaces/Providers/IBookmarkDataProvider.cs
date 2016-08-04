using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface IBookmarkDataProvider : IDataProvider<IBookmark>
    {
        IBookmark Read(string externalId, string bookmarkName);
        IBookmark Create(IWorkflowInstance workflowInstance, string bookmarkName, string ownerDisplayName, BookmarkStates state = BookmarkStates.Waiting, Nullable<DateTime> completionDate = null);
        void SetState(IBookmark bookmark, BookmarkStates state);
   }
}
