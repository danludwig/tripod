using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Tripod.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            IocConfig.Configure();
            ModelConfig.Configure();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // todo: should probably put the gravatar hash in a cookie to avoid hitting storage on every request (which means this would be needed again)
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            // Remove the "Server" HTTP Header from response
            var app = sender as HttpApplication;
            if (app == null || app.Context == null) return;

            var headers = app.Context.Response.Headers;
            if (headers["Server"] != null) headers.Remove("Server");
        }
    }
}
