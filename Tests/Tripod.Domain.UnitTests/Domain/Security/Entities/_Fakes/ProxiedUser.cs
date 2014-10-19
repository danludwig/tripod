using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedUser : User
    {
        protected internal ProxiedUser()
        {
            Name = "nameFromDb";
        }

        protected internal ProxiedUser(int id)
            : this()
        {
            Id = id;
        }

        private readonly ICollection<Permission> _permissions = new Collection<Permission>();
        public override ICollection<Permission> Permissions { get { return _permissions; } }

        public override LocalMembership LocalMembership { get; protected internal set; }

        private readonly ICollection<RemoteMembership> _remoteMemberships = new Collection<RemoteMembership>();
        public override ICollection<RemoteMembership> RemoteMemberships { get { return _remoteMemberships; } }

        private readonly ICollection<UserClaim> _claims = new Collection<UserClaim>();
        public override ICollection<UserClaim> Claims { get { return _claims; } }

        private readonly ICollection<EmailAddress> _emailAddresses = new Collection<EmailAddress>();
        public override ICollection<EmailAddress> EmailAddresses { get { return _emailAddresses; } }
    }
}