using Autofac;
using FlowBot.Common.Interfaces.Providers;
using FlowBot.Common.Interfaces.Services;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.Services
{
    public class IOCService : IIOCService, IDisposable
    {
        private ILifetimeScope _lifetimeScope;
        public IOCService(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope.BeginLifetimeScope();
        }
        protected void Dispose(bool disposing)
        {
            _lifetimeScope.Dispose();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        public T Resolve<T>()
        {
            return _lifetimeScope.Resolve<T>();
        }
       ~IOCService()
        {
            Dispose(false);
        }
    }
}
