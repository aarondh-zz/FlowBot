using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface IMessageDataProvider : IDataProvider<IMessage>
    {
        IMessage Create(IConversation conversation, IUser from, IUser to, string topic, string body, string locale);
    }
}
