using Autofac;
using Autofac.Integration.WebApi;
using FlowBot.Common.Interfaces.Providers;
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
    public class WebApiApplication : System.Web.HttpApplication, ILifetimeScopeProvider
    {
        private IContainer _iocContainer;
        public T BeginNewLifetimeScope<T>(object tag)
        {
            return (T)_iocContainer.BeginLifetimeScope(tag);
        }
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

            RegisterServices(builder);

            // Set the dependency resolver to be Autofac.
            _iocContainer = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(_iocContainer);

        }
        protected void RegisterServices(ContainerBuilder builder)
        {

            builder.RegisterInstance(this).As<ILifetimeScopeProvider>();

            builder.RegisterType<DataService>().As<IDataService>().InstancePerLifetimeScope();

            builder.RegisterType<WorkflowService>().As<IWorkflowService>().SingleInstance();

            builder.RegisterType<ConnectorService>().As<IConnectorService>().InstancePerLifetimeScope();

            builder.Register<LuisService>((context) =>
            {
                return new LuisService("386327ee-db6e-4042-a3db-3804724d980c", "cb244805c4144637bfadde5d4da230ec");
            }).As<ILuisService>().SingleInstance();

        }

        protected void Application_End()
        {
        }
    }
}
