using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Common.Interfaces.Providers
{
    public interface ILifetimeScopeProvider
    {
        T BeginNewLifetimeScope<T>(object tag);
    }
}
