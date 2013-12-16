using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;

namespace Tripod.Ioc.Security
{
    [ExcludeFromCodeCoverage]
    public class BigFatPhonyAuthenticationManager : IAuthenticationManager
    {
        public IEnumerable<AuthenticationDescription> GetAuthenticationTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AuthenticationDescription> GetAuthenticationTypes(Func<AuthenticationDescription, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticateResult> AuthenticateAsync(string authenticationType)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AuthenticateResult>> AuthenticateAsync(string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public void Challenge(AuthenticationProperties properties, params string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public void Challenge(params string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public void SignIn(AuthenticationProperties properties, params ClaimsIdentity[] identities)
        {
            throw new NotImplementedException();
        }

        public void SignIn(params ClaimsIdentity[] identities)
        {
            throw new NotImplementedException();
        }

        public void SignOut(params string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public ClaimsPrincipal User { get; set; }
        public AuthenticationResponseChallenge AuthenticationResponseChallenge { get; set; }
        public AuthenticationResponseGrant AuthenticationResponseGrant { get; set; }
        public AuthenticationResponseRevoke AuthenticationResponseRevoke { get; set; }
    }
}