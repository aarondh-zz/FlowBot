using Autofac;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
namespace FlowBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static IContainer IOCContainer;
        public static ILifetimeScope IOCApplicationLifetimeScope;
        [ThreadStatic]
        public static ILifetimeScope IOCRequestLifetimeScope;
        protected void Application_Start()
        {
#if NEVER
            var builder = new ContainerBuilder();

            //RegisterServices(builder);

            IOCContainer = builder.Build();

            IOCApplicationLifetimeScope = IOCContainer.BeginLifetimeScope();
#endif
            GlobalConfiguration.Configure(WebApiConfig.Register);

        }
#if NEVER
        protected void Application_BeginRequest()
        {
            //IOCRequestLifetimeScope = IOCApplicationLifetimeScope.BeginLifetimeScope();
        }

        protected void Application_EndRequest()
        {
            //IOCRequestLifetimeScope.Dispose();
        }
#endif
        protected void RegisterServices( ContainerBuilder builder )
        {
            builder.RegisterType<IOCService>().As<IIOCService>();

            builder.RegisterType<DataService>().As<IDataService>();

            builder.RegisterInstance(new LuisService("386327ee-db6e-4042-a3db-3804724d980c","cb244805c4144637bfadde5d4da230ec")).As<ILuisService>();

            builder.RegisterType<ConnectorService>().As<IConnectorService>();
        }

        protected void Application_End()
        {
            IOCApplicationLifetimeScope.Dispose();
        }
    }
}
