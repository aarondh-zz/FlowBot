using FlowBot.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Services
{
    public interface ILuisService
    {
        void Analyze(string text, IList<IIntent> intents, IList<IEntity> entities = null, double? minIntentScore = null, double? minEntityScore = null);
    }
}
