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
        void Reply(string text);
        Account GetFrom();
        string GetMessage();
        string GetChannelId();

        string GetConversationId();
        string GetConversationName();
    }
}
