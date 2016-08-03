using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using FlowBot.Common.Interfaces;
using FlowBot.Common.Models;
using FlowBot.Common.Interfaces.Services;

namespace FlowBotActivityLibrary
{

    public sealed class BotReceiveActivity : NativeActivity<string>
    {
        public OutArgument<string> Message { get; set; }
        public OutArgument<string> ConversationId { get; set; }
        public OutArgument<string> ConversationName { get; set; }
        public OutArgument<string> ChannelId { get; set; }
        public OutArgument<Account> From { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            context.CreateBookmark("message",
                       new BookmarkCallback(OnResumeBookmark));
        }
        protected override bool CanInduceIdle
        {
            get { return true; }
        }

        public void OnResumeBookmark(NativeActivityContext context, Bookmark bookmark, object connectorActivity)
        {
            var iocService = context.GetExtension<IIOCService>();
            var connectorService = iocService.Resolve<IConnectorService>();
            var message = connectorService.GetMessage(connectorActivity);
            context.SetValue<string>(this.Message, message);
            context.SetValue<string>(this.ConversationId, connectorService.GetConversationId(connectorActivity));
            context.SetValue<string>(this.ConversationName, connectorService.GetConversationName(connectorActivity));
            context.SetValue<string>(this.ChannelId, connectorService.GetChannelId(connectorActivity));
            context.SetValue<Account>(this.From, connectorService.GetFrom(connectorActivity));
            Result.Set(context, message);
        }
    }
}
