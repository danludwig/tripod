using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class PrincipalRemoteMembershipTicket : IDefineQuery<Task<RemoteMembershipTicket>>
    {
        public PrincipalRemoteMembershipTicket(IPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException("principal");
            Principal = principal;
        }

        public IPrincipal Principal { get; private set; }
    }

    [UsedImplicitly]
    public class HandlePrincipalRemoteMembershipTicketQuery : IHandleQuery<PrincipalRemoteMembershipTicket, Task<RemoteMembershipTicket>>
    {
        private readonly IAuthenticate _authenticator;

        public HandlePrincipalRemoteMembershipTicketQuery(IAuthenticate authenticator)
        {
            _authenticator = authenticator;
        }

        public Task<RemoteMembershipTicket> Handle(PrincipalRemoteMembershipTicket query)
        {
            return _authenticator.GetRemoteMembershipTicket(query.Principal);
        }
    }
}
