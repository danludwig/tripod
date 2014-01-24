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
            ConfigureAuth(app);
        }
    }
}
