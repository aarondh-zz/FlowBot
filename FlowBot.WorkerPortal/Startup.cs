using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FlowBot.WorkerPortal.Startup))]
namespace FlowBot.WorkerPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
