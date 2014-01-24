using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;
using Claim = System.Security.Claims.Claim;

namespace Tripod
{
    public interface IAuthenticate
    {
        Task SignOn(User user, bool isPersistent = false);
        Task SignOut();
        Task<RemoteMembershipTicket> GetRemoteMembershipTicket(IPrincipal principal);
        Task<IEnumerable<Claim>> GetRemoteMembershipClaims(string authenticationType = DefaultAuthenticationTypes.ExternalCookie);
    }
}
