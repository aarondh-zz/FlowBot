using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowBot.Utils
{
    public static class JsonUtils
    {
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        public static void SetGlobalJsonNetSettings()
        {
            JsonConvert.DefaultSettings = () => SerializerSettings;
        }
    }

}