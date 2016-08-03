using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Services.Models
{
    public class BookMarkResumedEventArgs : EventArgs
    {
        public string BookmarkName { get; private set; }

        public BookMarkResumedEventArgs(String bookmarkName)
        {
            this.BookmarkName = bookmarkName;
        }
    }
}
