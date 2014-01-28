using System.Web.Mvc;
using SimpleInjector;

namespace Tripod.Web
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, Container container)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticatedUserIdAttribute());

            // todo: should probably put the gravatar hash in a cookie to avoid hitting storage on every request
            filters.Add(new AuthenticatedUserGravatarAttribute
            {
                Queries = container.GetInstance<IProcessQueries>(),
            });
        }
    }
}
