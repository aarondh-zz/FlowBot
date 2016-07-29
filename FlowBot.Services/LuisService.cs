using FlowBot.Common.Interfaces;
using FlowBot.Common.Interfaces.Models;
using FlowBot.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Services
{
    public class LuisService : ILuisService
    {
        private Microsoft.Bot.Builder.Luis.ILuisService _luisService;

        private class Intent : IIntent
        {
            public double? Score { get; }

            public string Text { get; }

            public Intent(string text, double? score)
            {
                this.Text = text;
                this.Score = score;
            }
        }

        private class Entity : IEntity
        {
            public double? Score { get; }

            public string Text { get; }

            public Entity(string text, double? score)
            {
                this.Text = text;
                this.Score = score;
            }
        }

        public LuisService(string modelId, string subscriptionKey)
        {
            _luisService = new Microsoft.Bot.Builder.Luis.LuisService(new Microsoft.Bot.Builder.Luis.LuisModelAttribute(modelId,subscriptionKey));
        }

        public void Analyze(string text, IList<IIntent> intents, IList<IEntity> entities = null, double? minIntentScore = null, double? minEntityScore = null)
        {
            Uri uri = _luisService.BuildUri(text);
            var task = _luisService.QueryAsync(uri);
            var luisResult = task.Result;
            if (intents != null)
            {
                foreach (var intent in luisResult.Intents)
                {
                    if (minIntentScore.HasValue && intent.Score.HasValue && intent.Score.Value >= minIntentScore.Value)
                    {
                        intents.Add(new Intent(intent.Intent,intent.Score));
                    }
                }
            }
            if (entities != null)
            {
                foreach (var entity in luisResult.Entities)
                {
                    if (minEntityScore.HasValue && entity.Score.HasValue && entity.Score.Value >= minEntityScore.Value)
                    {
                        entities.Add(new Entity(entity.Entity, entity.Score));
                    }
                }
            }
        }
    }
}
