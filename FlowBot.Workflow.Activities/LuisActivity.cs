using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Common.Interfaces.Models;

namespace FlowBotActivityLibrary
{

    public sealed class LuisActivity : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<string> Text { get; set; }
        public InArgument<double> MinScore { get; set; }
        public OutArgument<List<IIntent>> Intents { get; set; }
        public OutArgument<List<IEntity>> Entities { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {

            string text = context.GetValue(this.Text);

            double minScore = context.GetValue<double>(this.MinScore);

            var iocService = context.GetExtension<IIOCService>();
            var luisService = iocService.Resolve<ILuisService>();
            if (luisService == null)
            {
                throw new NotSupportedException(typeof(ILuisService).FullName + " extension was not found");
            }

            List<IIntent> intents = new List<IIntent>();
            List<IEntity> entities = new List<IEntity>();

            luisService.Analyze(context.GetValue<string>(this.Text), intents, entities, minScore);

            context.SetValue<List<IIntent>>(this.Intents, intents);
            context.SetValue<List<IEntity>>(this.Entities, entities);
        }
    }
}
