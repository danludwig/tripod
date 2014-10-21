using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Retrieve all Claims from a cookie (defaults to external cookie).
    /// </summary>
    public class ExternalCookieClaims : IDefineQuery<Task<IEnumerable<Claim>>>
    {
        /// <summary>
        /// Retrieve all Claims from a cookie (defaults to external cookie).
        /// </summary>
        /// <param name="authenticationType">Type of cookie, default is external when omitted.</param>
        public ExternalCookieClaims(string authenticationType = DefaultAuthenticationTypes.ExternalCookie)
        {
            AuthenticationType = authenticationType;
        }

        public string AuthenticationType { get; private set; }
    }

    [UsedImplicitly]
    public class HandleExternalCookieClaimsQuery : IHandleQuery<ExternalCookieClaims, Task<IEnumerable<Claim>>>
    {
        private readonly IAuthenticate _authenticator;

        public HandleExternalCookieClaimsQuery(IAuthenticate authenticator)
        {
            _authenticator = authenticator;
        }

        public async Task<IEnumerable<Claim>> Handle(ExternalCookieClaims query)
        {
            return await _authenticator
                .GetRemoteMembershipClaims(query.AuthenticationType)
                .ConfigureAwait(false)
            ;
        }
    }
}
