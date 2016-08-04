using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Services;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBotActivityLibrary
{
    public class BotReplyActivity : NativeActivity<string>
    {
        [DefaultValue(null)]
        public InArgument<string> Text { get; set; }
        public InArgument<string> Locale { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            var iocService = context.GetExtension<IIOCService>();
            var connectorService = iocService.Resolve<IConnectorService>();
            if (connectorService == null)
            {
                throw new NotSupportedException(typeof(IConnectorService).FullName + " extension was not found");
            }
            connectorService.Reply(context.GetValue<string>(this.Text), context.GetValue<string>(this.Locale));
        }
    }
}
