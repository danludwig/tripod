using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using SimpleInjector;
using Tripod.Domain.Security;

namespace Tripod.Services.Security
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

            // identity UserManager<User, int> registration
            container.Register(() => new UserManager<User, int>(container.GetInstance<IUserStore<User, int>>()));

            container.Register<IUserStore<UserTicket, string>, UserTokenSecurityStore>();
            container.Register(() =>
            {
                var userManager = new UserManager<UserTicket, string>(container.GetInstance<IUserStore<UserTicket, string>>());
                var protectionProvider = container.GetInstance<IDataProtectionProvider>();
                var purposes = new[]
                {
                    EmailVerificationPurpose.CreateLocalUser.ToString(),
                    EmailVerificationPurpose.CreateRemoteUser.ToString(),
                    EmailVerificationPurpose.AddEmail.ToString(),
                    EmailVerificationPurpose.ForgotPassword.ToString(),
                };
                IDataProtector dataProtector = protectionProvider.Create(purposes);
                userManager.UserTokenProvider = new DataProtectorTokenProvider<UserTicket, string>(dataProtector);
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