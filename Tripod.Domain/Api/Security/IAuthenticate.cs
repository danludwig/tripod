using System.Security.Principal;
using System.Threading.Tasks;
using Tripod.Domain.Security;

namespace Tripod
{
    public interface IAuthenticate
    {
        Task SignOn(User user, bool isPersistent = false);
        Task SignOut();
        Task<RemoteMembershipTicket> GetRemoteMembershipTicket(IPrincipal principal);
    }
}
