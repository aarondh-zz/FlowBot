using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Data
{
    public partial class Message : IMessage
    {
        IConversation IMessage.Conversation
        {
            get
            {
                return this.Conversation;
            }
        }

        IUser IMessage.To
        {
            get
            {
                return this.To;
            }
        }
        IUser IMessage.From
        {
            get
            {
                return this.From;
            }
        }
    }
}
