using System.Web;
using Microsoft.AspNet.Identity;
using SimpleInjector;
using Tripod.Domain.Security;

namespace Tripod.Ioc.Security
{
    public static class CompositionRoot
    {
        public static void RegisterSecurity(this Container container)
        {
            container.Register<SecurityStore>();
            container.Register<IUserStore<User, int>, SecurityStore>();
            container.Register<IUserLoginStore<User, int>, SecurityStore>();
            container.Register<IUserRoleStore<User, int>, SecurityStore>();
            container.Register<IUserPasswordStore<User, int>, SecurityStore>();
            container.Register<IUserClaimStore<User, int>, SecurityStore>();
            container.Register<IUserSecurityStampStore<User, int>, SecurityStore>();
            container.Register<IQueryableUserStore<User, int>, SecurityStore>();
            container.Register<IUserConfirmationStore<User, int>, SecurityStore>();
            container.Register<IUserEmailStore<User, int>, SecurityStore>();

            container.Register(() => HttpContext.Current != null && HttpContext.Current.Items["owin.Environment"] != null
                ? HttpContext.Current.GetOwinContext().Authentication : new BigFatPhonyAuthenticationManager());

            container.Register<IAuthenticate, OwinAuthenticationManager>();

            container.Register<UserManager<User, int>>();
        }
    }
}
