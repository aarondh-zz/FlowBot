using Autofac;
using Autofac.Integration.WebApi;
using FlowBot.Common.Interfaces.Providers;
using FlowBot.Common.Interfaces.Services;
using FlowBot.Services;
using FlowBot.Utils;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Web.Http;
namespace FlowBot
{
    public class WebApiApplication : System.Web.HttpApplication, ILifetimeScopeProvider
    {
        private IContainer _iocContainer;
        public T BeginNewLifetimeScope<T>(object tag)
        {
            return (T)_iocContainer.BeginLifetimeScope(tag);
        }
        public T BeginNewLifetimeScope<T,B>(object tag, Action<B> configurationAction)
        {
            return (T)_iocContainer.BeginLifetimeScope(tag, (containerBuilder)=>
            {
                configurationAction((B)(object)containerBuilder);
            });
        }
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            RegisterServices(builder);

            // Set the dependency resolver to be Autofac.
            _iocContainer = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(_iocContainer);

            JsonUtils.SetGlobalJsonNetSettings();


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
