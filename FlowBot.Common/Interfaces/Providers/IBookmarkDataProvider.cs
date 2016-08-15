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
        IBookmark Read(Guid workflowInstanceId, string bookmarkName, BookmarkStates state = BookmarkStates.Undefined);
        IBookmark Create(IWorkflowInstance workflowInstance, string bookmarkName, string ownerDisplayName, BookmarkStates state = BookmarkStates.Waiting, Nullable<DateTime> completionDate = null);
        void SetState(IBookmark bookmark, BookmarkStates state);
        IOrderedQueryable<IBookmark> List(string bookmarkName = null, Guid? instanceId = null, string ownerDisplayName = null, BookmarkStates? state = null, OrderBy orderBy = OrderBy.OldestToNewest);
        int CancelAllWaiting(Guid workflowInstanceId);
   }
}
