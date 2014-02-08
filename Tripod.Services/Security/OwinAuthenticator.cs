using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Tripod.Domain.Security;

namespace Tripod.Services.Security
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

        public Task SignOut()
        {
            ThrowIfNoOwin();
            _authenticationManager.SignOut();
            return Task.FromResult(0);
        }

        public async Task<RemoteMembershipTicket> GetRemoteMembershipTicket(IPrincipal principal)
        {
            ThrowIfNoOwin();
            ExternalLoginInfo info;
            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                info = await _authenticationManager.GetExternalLoginInfoAsync();
            }
            else
            {
                var xsrfKey = AppConfiguration.XsrfKey;
                info = await _authenticationManager.GetExternalLoginInfoAsync(xsrfKey, principal.Identity.GetUserId());
            }

            if (info == null) return null;
            return new RemoteMembershipTicket
            {
                Login = info.Login,
                UserName = info.DefaultUserName,
            };
        }

        public async Task<IEnumerable<Claim>> GetRemoteMembershipClaims(string authenticationType = DefaultAuthenticationTypes.ExternalCookie)
        {
            var claimsIdentity = await _authenticationManager.GetExternalIdentityAsync(authenticationType);
            return claimsIdentity.Claims;
        }

        private void ThrowIfNoOwin()
        {
            if (_authenticationManager == null || _authenticationManager is BigFatPhonyAuthenticationManager)
                throw new InvalidOperationException("There is no owin environment in this context.");
        }
    }
}
