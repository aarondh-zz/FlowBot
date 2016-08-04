using FlowBot.Common.Interfaces.Models;
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
        void BindActivity(IWorkflowHandle workflowHandle, object activity);
        void Reply(string text, string locale = null);
        Account GetFrom();
        string GetTopic();
        string GetLocale();
        string GetMessage();
        string GetChannelId();

        string GetConversationId();
        string GetConversationName();
        Account GetFrom(object activity);
        string GetMessage(object activity);
        string GetLocale(object activity);
        string GetTopic(object activity);
        string GetChannelId(object activity);

        string GetConversationId(object activity);
        string GetConversationName(object activity);
    }
}
