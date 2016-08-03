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

    public sealed class BotDialog : NativeActivity<string>
    {
        // Define an activity input argument of type string
        public OutArgument<string> Message { get; set; }
        public OutArgument<string> ConversationId { get; set; }
        public OutArgument<string> ConversationName { get; set; }
        public OutArgument<string> ChannelId { get; set; }
        public OutArgument<Account> From { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var iocService = context.GetExtension<IIOCService>();
            var connectorService = iocService.Resolve<IConnectorService>();
            if ( connectorService == null)
            {
                throw new NotSupportedException(typeof(IConnectorService).FullName + " extension was not found");
            }
            var message = connectorService.GetMessage();
            context.SetValue<string>(this.Message, message);
            context.SetValue<string>(this.ConversationId, connectorService.GetConversationId());
            context.SetValue<string>(this.ConversationName, connectorService.GetConversationName());
            context.SetValue<string>(this.ChannelId, connectorService.GetChannelId());
            context.SetValue<Account>(this.From, connectorService.GetFrom());

            Result.Set(context, message);
        }
    }
}
