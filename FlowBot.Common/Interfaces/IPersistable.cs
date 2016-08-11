using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlowBot.Common.Interfaces
{
    public interface IPersistable
    {
        XName GetPersistablePropertyName();
        string Dehydrate();
        void Rehydrate(string value);
    }
}
