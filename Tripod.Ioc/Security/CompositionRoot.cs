using System.Web;
using Microsoft.AspNet.Identity;
using Owin;
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
            //container.Register<IUserConfirmationStore<User, int>, SecurityStore>();
            //container.Register<IUserEmailStore<User, int>, SecurityStore>();

            // identity UserManager<User, int> registration
            container.Register(() =>
            {
                var userManager = new UserManager<User, int>(container.GetInstance<IUserStore<User, int>>());
                if (!HasOwinContext()) return userManager;

                // the owin context user manager factory has our token provider
                var owinContext = HttpContext.Current.GetOwinContext();
                var owinUserManager = owinContext.GetUserManager<UserManager<User, int>>();
                userManager.UserConfirmationTokens = owinUserManager.UserConfirmationTokens;
                userManager.PasswordResetTokens = owinUserManager.PasswordResetTokens;
                return userManager;
            });

            // owin IAuthenticationManager registration
            container.Register(() => HasOwinContext()
                ? HttpContext.Current.GetOwinContext().Authentication : new BigFatPhonyAuthenticationManager());

            container.Register<IAuthenticate, OwinAuthenticator>();
        }

        private static bool HasOwinContext()
        {
            return HttpContext.Current != null && HttpContext.Current.Items["owin.Environment"] != null;
        }
    }
}
