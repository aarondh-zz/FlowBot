using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowBot.Json
{
    public static class JsonUtils
    {
        public static readonly JsonSerializerSettings SerializerSettings = Configure(new JsonSerializerSettings());
        public static JsonSerializerSettings Configure( JsonSerializerSettings settings)
        {
            settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            settings.Formatting = Formatting.None;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return settings;
        }
        public static void SetGlobalJsonNetSettings()
        {
            JsonConvert.DefaultSettings = () => SerializerSettings;
        }
    }

}