using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Services
{
    public class ConnectorService : IConnectorService
    {
        public delegate ConnectorService Factory(Activity activity);
        public void BindActivity(object activity)
        {
            _activity = activity as Activity;
            _connector = new ConnectorClient(new Uri(_activity.ServiceUrl));
        }
        private Activity _activity;
        private ConnectorClient _connector;
        public ConnectorService()
        {
        }

        public void Reply(string text)
        {
            Activity reply = _activity.CreateReply(text);
            _connector.Conversations.ReplyToActivityAsync(reply);
        }

        public string GetMessage()
        {
            return _activity.Text;
        }
        public Account GetFrom()
        {
            return new Account() { Name = _activity.From.Name, Id = _activity.From.Id };
        }
        public string GetChannelId()
        {
            return _activity.ChannelId;
        }

        public string GetConversationId()
        {
            return _activity.Conversation.Id;
        }
        public string GetConversationName()
        {
            return _activity.Conversation.Name;
        }
    }
}
