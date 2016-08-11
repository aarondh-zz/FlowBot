using FlowBot.Common.Interfaces;
using System;
using System.Activities.Persistence;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlowBot.Services
{
    public abstract class PersistableServiceBase : PersistenceParticipant, IPersistable
    {
        public static readonly XNamespace PersistableNamespace = XNamespace.Get("http://daisley-harrison.com/namespaces/flowbot");
        public static XName CreatePersistablePropertyName(string name)
        {
            return PersistableNamespace.GetName(name);
        }
        public abstract XName GetPersistablePropertyName();
        protected override void CollectValues(out IDictionary<XName, object> readWriteValues,
                          out IDictionary<XName, object> writeOnlyValues)
        {
            readWriteValues = new Dictionary<XName, object>();
            var hydrate = this.Dehydrate();
            Debug.WriteLine($"dehydrating {GetType()}: {hydrate}");
            readWriteValues.Add(GetPersistablePropertyName(), hydrate);
            writeOnlyValues = null;
        }
        protected override IDictionary<XName, object> MapValues(IDictionary<XName, object> readWriteValues,
                      IDictionary<XName, object> writeOnlyValues)
        {
            return base.MapValues(readWriteValues, writeOnlyValues);
        }
        protected override void PublishValues(IDictionary<XName, object> readWriteValues)
        {
            var hydrate = readWriteValues[GetPersistablePropertyName()] as string;
            Debug.WriteLine($"rehydrating {GetType()}: {hydrate}");
            this.Rehydrate(hydrate);
        }
        public abstract string Dehydrate();

        public abstract void Rehydrate(string value);
    }
}
