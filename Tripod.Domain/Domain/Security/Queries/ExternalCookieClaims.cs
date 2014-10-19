using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class ExternalCookieClaims : IDefineQuery<Task<IEnumerable<Claim>>>
    {
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
