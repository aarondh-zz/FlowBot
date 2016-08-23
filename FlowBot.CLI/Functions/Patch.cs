using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI.Models
{
    public enum PatchOperators
    {
        Add,
        Remove,
        Replace
    }
    public class Patch
    {
        [JsonProperty(PropertyName = "op")]
        public string Op { get; set; }
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }
        public Patch()
        {

        }
        public Patch(PatchOperators op, string path, object value)
        {
            switch(op)
            {
                case PatchOperators.Add:
                    this.Op = "add";
                    break;
                case PatchOperators.Remove:
                    this.Op = "remove";
                    break;
                case PatchOperators.Replace:
                    this.Op = "replace";
                    break;
            }
            this.Path = path;
            this.Value = value;
        }
    }
}
