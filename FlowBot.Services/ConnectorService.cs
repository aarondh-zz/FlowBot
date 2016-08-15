using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlowBot.Services
{
    public class ConnectorService : PersistableServiceBase, IConnectorService
    {
        private static readonly XName PersistablePropertyName = PersistableServiceBase.CreatePersistablePropertyName("connectorService");
        private IDataService _dataService;
        private string _serviceUrl;
        private string _channelConversationId;
        private string _channelId;
        private string _topicName;
        private string _locale;
        private IUser _user;
        private IUser _bot;
        private ChannelAccount _userChannelAccount;
        private ChannelAccount _botChannelAccount;
        private IConversation _conversation;
        private Guid _conversationId;
        public delegate ConnectorService Factory(Activity activity);
        public void BindActivity(IWorkflowHandle workflowHandle, object activity)
        {
            _activity = activity as Activity;
            _serviceUrl = _activity.ServiceUrl;
            _locale = _activity.Locale;
            _topicName = _activity.TopicName;
            _userChannelAccount = _activity.From;
            _botChannelAccount = _activity.Recipient;
            _channelId = _activity.ChannelId;
            _channelConversationId = _activity.Conversation.Id;
            _user = _dataService.Users.ReadOrCreate("User", _userChannelAccount.Id, _userChannelAccount.Name);
            _bot = _dataService.Users.ReadOrCreate("Bot", _botChannelAccount.Id, _botChannelAccount.Name);
            _conversation = _dataService.Conversations.ReadOrCreate(workflowHandle, _channelConversationId);
            _conversationId = _conversation.Id;
            _dataService.Messages.Create(_conversation, _user, _bot, _activity.TopicName, _activity.Text, _activity.Locale);
            _connector = new ConnectorClient(new Uri(_serviceUrl));
        }
        private Activity _activity;
        private ConnectorClient _connector;
        public ConnectorService(IDataService dataService)
        {
            _dataService = dataService;
        }
        public override XName GetPersistablePropertyName()
        {
            return PersistablePropertyName;
        }
        public override string Dehydrate()
        {
            return JsonConvert.SerializeObject(new
            {
                serviceUrl = _serviceUrl,
                channelId = _channelId,
                channelConversationId = _channelConversationId,
                conversationId = _conversationId,
                locale = _locale,
                topicName = _topicName,
                user = new 
                {
                    id = _userChannelAccount.Id,
                    name = _userChannelAccount.Name
                },
                bot = new
                {
                    id = _botChannelAccount.Id,
                    name = _botChannelAccount.Name
                }
            });
        }
        public override void Rehydrate(string value)
        {
            dynamic rehydrated = JsonConvert.DeserializeObject(value);
            _serviceUrl = rehydrated.serviceUrl;
            _channelId = rehydrated.channelId;
            _channelConversationId = rehydrated.channelConversationId;
            _conversationId = rehydrated.conversationId;
            _locale = rehydrated.locale;
            _topicName = rehydrated.topicName;
            var user = new Account()
            {
                Id = rehydrated.user.id,
                Name = rehydrated.user.name
            };
            var bot = new Account()
            {
                Id = rehydrated.bot.id,
                Name = rehydrated.bot.name
            };
            _userChannelAccount = new ChannelAccount(user.Id, user.Name);
            _botChannelAccount = new ChannelAccount(bot.Id, bot.Name);
            _user = _dataService.Users.ReadOrCreate("User", _userChannelAccount.Id, _userChannelAccount.Name);
            _bot = _dataService.Users.ReadOrCreate("Bot", _botChannelAccount.Id, _botChannelAccount.Name);
            _conversation = _dataService.Conversations.Read(_conversationId);
            _connector = new ConnectorClient(new Uri(_serviceUrl));
        }
        public bool IsExistingConversionation
        {
            get
            {
                return _activity != null;
            }
        }

        public void Reply(string text, string locale = null)
        {
            if (locale == null)
            {
                locale = GetLocale();
            }
            if (this.IsExistingConversionation)
            {
                Activity reply = _activity.CreateReply(text, locale);
                _connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                Send(text, locale);
            }
            _dataService.Messages.Create(_conversation, _bot, _user, _topicName, text, locale);
        }

        public void Send(string text, string locale = null)
        {
            if (locale == null)
            {
                locale = GetLocale();
            }
            
            IMessageActivity message = Activity.CreateMessageActivity();

            message.From = _botChannelAccount;

            message.Recipient = _userChannelAccount;

            message.Conversation = new ConversationAccount(id: _channelConversationId);

            message.Text = text;

            message.Locale = locale;

            _connector.Conversations.SendToConversation((Activity)message);
        }
        public void StartConverstationAndSend(string text, string locale = null)
        {
            if (locale == null)
            {
                locale = GetLocale();
            }

            var resourceResponse = _connector.Conversations.CreateDirectConversation(_botChannelAccount, _userChannelAccount);

            IMessageActivity message = Activity.CreateMessageActivity();

            message.From = _botChannelAccount;

            message.Recipient = _userChannelAccount;

            message.Conversation = new ConversationAccount(id: resourceResponse.Id);

            message.Text = text;

            message.Locale = locale;

            _connector.Conversations.SendToConversation((Activity)message);
        }

        public string GetMessage()
        {
            return _activity?.Text;
        }
        public string GetTopic()
        {
            return _activity?.TopicName;
        }
        public string GetLocale()
        {
            return _locale;
        }
        public Account GetFrom()
        {
            return new Account() { Name = _activity.From.Name, Id = _activity.From.Id };
        }
        public string GetChannelId()
        {
            return _channelId;
        }
        public string GetServiceUrl()
        {
            return _serviceUrl;
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

        public string GetServiceUrl(object activity)
        {
            return ((Activity)activity).ServiceUrl;
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
