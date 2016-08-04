using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Models;
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
        private IDataService _dataService;
        private IUser _from;
        private IUser _to;
        private IConversation _conversation;
        public delegate ConnectorService Factory(Activity activity);
        public void BindActivity(IWorkflowHandle workflowHandle, object activity)
        {
            _activity = activity as Activity;
            _connector = new ConnectorClient(new Uri(_activity.ServiceUrl));
            _from = _dataService.Users.ReadOrCreate("User", _activity.From.Id, _activity.From.Name);
            _to = _dataService.Users.ReadOrCreate("Bot", _activity.Recipient.Id, _activity.Recipient.Name);
            _conversation = _dataService.Conversations.ReadOrCreate(workflowHandle, _activity.Conversation.Id);
            _dataService.Messages.Create(_conversation, _from, _to, _activity.TopicName, _activity.Text, _activity.Locale);
        }
        private Activity _activity;
        private ConnectorClient _connector;
        public ConnectorService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void Reply(string text, string locale = null)
        {
            if ( locale == null)
            {
                locale = _activity.Locale;
            }
            Activity reply = _activity.CreateReply(text, locale);
            _connector.Conversations.ReplyToActivityAsync(reply);
            _dataService.Messages.Create(_conversation, _to, _from, _activity.TopicName, text, locale);
        }

        public string GetMessage()
        {
            return _activity.Text;
        }
        public string GetTopic()
        {
            return _activity.TopicName;
        }
        public string GetLocale()
        {
            return _activity.Locale;
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
        public string GetMessage(object activity)
        {
            return ((Activity)activity).Text;
        }
        public string GetLocale(object activity)
        {
            return ((Activity)activity).Locale;
        }
        public string GetTopic(object activity)
        {
            return ((Activity)activity).TopicName;
        }
        public Account GetFrom(object activity)
        {
            return new Account() { Name = ((Activity)activity).From.Name, Id = ((Activity)activity).From.Id };
        }
        public string GetChannelId(object activity)
        {
            return ((Activity)activity).ChannelId;
        }

        public string GetConversationId(object activity)
        {
            return ((Activity)activity).Conversation.Id;
        }
        public string GetConversationName(object activity)
        {
            return ((Activity)activity).Conversation.Name;
        }
    }
}
