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

    public sealed class BotDialog : CodeActivity
    {
        // Define an activity input argument of type string
        public OutArgument<string> Message { get; set; }
        public OutArgument<string> ConversationId { get; set; }
        public OutArgument<string> ConversationName { get; set; }
        public OutArgument<string> ChannelId { get; set; }
        public OutArgument<Account> From { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var connectorService = context.GetExtension<IConnectorService>();
            if ( connectorService == null)
            {
                throw new NotSupportedException(typeof(IConnectorService).FullName + " extension was not found");
            }
            context.SetValue<string>(this.Message, connectorService.GetMessage());
            context.SetValue<string>(this.ConversationId, connectorService.GetConversationId());
            context.SetValue<string>(this.ConversationName, connectorService.GetConversationName());
            context.SetValue<string>(this.ChannelId, connectorService.GetChannelId());
            context.SetValue<Account>(this.From, connectorService.GetFrom());
        }
    }
}
