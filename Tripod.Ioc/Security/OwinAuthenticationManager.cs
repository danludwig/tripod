using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Tripod.Domain.Security;

namespace Tripod.Ioc.Security
{
    public class OwinAuthenticationManager : IAuthenticate
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly UserManager<User, int> _userManager;

        public OwinAuthenticationManager(IAuthenticationManager authenticationManager, UserManager<User, int> userManager)
        {
            _authenticationManager = authenticationManager;
            _userManager = userManager;
        }

        public async Task SignOn(User user, bool isPersistent = false)
        {
            ThrowIfNoOwin();
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
        }

        public Task SignOff()
        {
            ThrowIfNoOwin();
            _authenticationManager.SignOut();
            return Task.FromResult(0);
        }

        private void ThrowIfNoOwin()
        {
            if (_authenticationManager == null || _authenticationManager is BigFatPhonyAuthenticationManager)
                throw new InvalidOperationException("There is no owin environment in this context.");
        }
    }
}
