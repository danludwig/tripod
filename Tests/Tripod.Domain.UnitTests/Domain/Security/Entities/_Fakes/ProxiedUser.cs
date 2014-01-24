using System.Collections.Generic;
using System.Linq;

namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedUser : User
    {
        protected internal ProxiedUser()
        {
            Name = "nameFromDb";
        }

        public override ICollection<Permission> Permissions
        {
            get { return base.Permissions.ToArray(); }
        }

        public override LocalMembership LocalMembership { get; protected internal set; }

        public override ICollection<RemoteMembership> RemoteMemberships
        {
            get { return base.RemoteMemberships.ToArray(); }
        }

        public override ICollection<UserClaim> Claims
        {
            get { return base.Claims.ToArray(); }
        }

        public override ICollection<EmailAddress> EmailAddresses
        {
            get { return base.EmailAddresses.ToArray(); }
        }
    }
}