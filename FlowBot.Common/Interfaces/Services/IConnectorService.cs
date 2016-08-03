using FlowBot.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Services
{
    public interface IConnectorService
    {
        void BindActivity(object activity);
        void Reply(string text);
        Account GetFrom();
        string GetMessage();
        string GetChannelId();

        string GetConversationId();
        string GetConversationName();
        Account GetFrom(object activity);
        string GetMessage(object activity);
        string GetChannelId(object activity);

        string GetConversationId(object activity);
        string GetConversationName(object activity);
    }
}
