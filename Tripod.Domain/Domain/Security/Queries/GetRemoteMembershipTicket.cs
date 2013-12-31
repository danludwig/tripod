using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class GetRemoteMembershipTicket : IDefineQuery<Task<RemoteMembershipTicket>>
    {
        public GetRemoteMembershipTicket(IPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException("principal");
            Principal = principal;
        }
        public IPrincipal Principal { get; private set; }
    }

    public class HandleGetRemoteMembershipTicketQuery : IHandleQuery<GetRemoteMembershipTicket, Task<RemoteMembershipTicket>>
    {
        private readonly IAuthenticate _authenticator;

        public HandleGetRemoteMembershipTicketQuery(IAuthenticate authenticator)
        {
            _authenticator = authenticator;
        }

        public Task<RemoteMembershipTicket> Handle(GetRemoteMembershipTicket query)
        {
            return _authenticator.GetRemoteMembershipTicket(query.Principal);
        }
    }
}
