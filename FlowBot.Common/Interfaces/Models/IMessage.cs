using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Models
{
    public interface IMessage : IRecord
    {
        IUser From { get; }
        IUser To { get; }
        string Topic { get; }
        string Body { get; }
        string Locale { get; }
        IConversation Conversation { get; }
    }
}
