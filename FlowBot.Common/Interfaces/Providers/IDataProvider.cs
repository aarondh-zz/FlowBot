using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface IDataProvider<T>
    {
        T Create(T obj);
        T Read(string id);
        void Update(T obj);
        void Delete(T obj);
        IEnumerable<T> List();
    }
}
