using System.Web.Mvc;
using System.Web.Routing;

namespace Tripod.Web
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.LowercaseUrls = true; // this makes the parameters lowercase too
            routes.AppendTrailingSlash = true;

            routes.MapMvcAttributeRoutes();

            //routes.MapRoute(
            //    name: null,
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
