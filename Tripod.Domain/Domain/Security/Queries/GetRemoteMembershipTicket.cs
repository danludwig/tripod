using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class GetRemoteMembershipTicket : IDefineQuery<Task<RemoteMembershipTicket>>
    {
        public GetRemoteMembershipTicket() { }

        public GetRemoteMembershipTicket(IPrincipal principal, string xsrfKey)
        {
            if (principal == null)
                throw new ArgumentNullException("principal");
            if (string.IsNullOrWhiteSpace(xsrfKey))
                throw new ArgumentException(Resources.Exception_Argument_CannotBeNullOrEmpty, "xsrfKey");
            XsrfKey = xsrfKey;
            Principal = principal;
        }
        public IPrincipal Principal { get; private set; }
        public string XsrfKey { get; private set; }
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
            return string.IsNullOrWhiteSpace(query.XsrfKey)
                ? _authenticator.GetRemoteMembershipTicket()
                : _authenticator.GetRemoteMembershipTicket(query.Principal, query.XsrfKey);
        }
    }
}
