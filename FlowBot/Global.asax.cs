using Autofac;
using Autofac.Integration.WebApi;
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
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            //RegisterServices(builder);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

        }
        protected void RegisterServices( ContainerBuilder builder )
        {
            builder.RegisterType<IOCService>().As<IIOCService>();

            builder.RegisterType<DataService>().As<IDataService>();

            builder.Register<LuisService>((context) =>
            {
                return new LuisService("386327ee-db6e-4042-a3db-3804724d980c", "cb244805c4144637bfadde5d4da230ec");
            }).As<ILuisService>().SingleInstance();

            builder.RegisterType<ConnectorService>().As<IConnectorService>();
        }

        protected void Application_End()
        {
        }
    }
}
