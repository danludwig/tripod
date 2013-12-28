using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Tripod.Domain.Security;

namespace Tripod.Ioc.Security
{
    public class OwinAuthenticator : IAuthenticate
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly UserManager<User, int> _userManager;

        public OwinAuthenticator(IAuthenticationManager authenticationManager, UserManager<User, int> userManager)
        {
            _authenticationManager = authenticationManager;
            _userManager = userManager;
        }

        public Task SignOn(User user, bool isPersistent = false)
        {
            ThrowIfNoOwin();
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie).Result;
            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
            return Task.FromResult(0);
        }

        public Task SignOff()
        {
            ThrowIfNoOwin();
            _authenticationManager.SignOut();
            return Task.FromResult(0);
        }

        public async Task<RemoteMembershipTicket> GetRemoteMembershipTicket()
        {
            ThrowIfNoOwin();
            return GetRemoteMembershipTicket(await _authenticationManager.GetExternalLoginInfoAsync());
        }

        public async Task<RemoteMembershipTicket> GetRemoteMembershipTicket(IPrincipal principal, string xsrfKey)
        {
            ThrowIfNoOwin();
            return GetRemoteMembershipTicket(await _authenticationManager.GetExternalLoginInfoAsync(xsrfKey, principal.Identity.GetUserId()));
        }

        private static RemoteMembershipTicket GetRemoteMembershipTicket(ExternalLoginInfo externalLoginInfo)
        {
            if (externalLoginInfo == null) return null;
            return new RemoteMembershipTicket
            {
                Login = externalLoginInfo.Login,
                UserName = externalLoginInfo.DefaultUserName,
            };
        }

        private void ThrowIfNoOwin()
        {
            if (_authenticationManager == null || _authenticationManager is BigFatPhonyAuthenticationManager)
                throw new InvalidOperationException("There is no owin environment in this context.");
        }
    }
}
