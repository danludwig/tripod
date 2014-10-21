using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Retrieve a specific claim from a cookie (defaults to external cookie).
    /// </summary>
    public class ExternalCookieClaim : IDefineQuery<Task<Claim>>
    {
        /// <summary>
        /// Retrieve a specific claim from a cookie (defaults to external cookie).
        /// </summary>
        /// <param name="claimType">Claim type to find in the cookie.</param>
        /// <param name="authenticationType">Type of cookie, default is external when omitted.</param>
        public ExternalCookieClaim(string claimType, string authenticationType = DefaultAuthenticationTypes.ExternalCookie)
        {
            ClaimType = claimType;
            AuthenticationType = authenticationType;
        }

        public string ClaimType { get; private set; }
        public string AuthenticationType { get; private set; }
    }

    [UsedImplicitly]
    public class HandleExternalCookieClaimQuery : IHandleQuery<ExternalCookieClaim, Task<Claim>>
    {
        private readonly IProcessQueries _queries;

        public HandleExternalCookieClaimQuery(IProcessQueries queries)
        {
            _queries = queries;
        }

        public async Task<Claim> Handle(ExternalCookieClaim query)
        {
            var claims = await _queries.Execute(new ExternalCookieClaims(query.AuthenticationType))
                .ConfigureAwait(false);
            return claims.FirstOrDefault(x => x.Type == query.ClaimType);
        }
    }
}
