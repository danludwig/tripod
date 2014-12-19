using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tripod.Web.Startup))]
namespace Tripod.Web
{
    public partial class Startup
    {
        [UsedImplicitly]
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR(new HubConfiguration
            {
                EnableDetailedErrors = true,
            });
            ConfigureAuth(app);
        }
    }
}
