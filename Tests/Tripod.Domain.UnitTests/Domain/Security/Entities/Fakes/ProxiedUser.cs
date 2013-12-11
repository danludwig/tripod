using System.Collections.Generic;

namespace Tripod.Domain.Security
{
    public class ProxiedUser : User
    {
        protected internal ProxiedUser()
        {
            Name = "nameFromDb";
        }

        public override ICollection<Permission> Permissions { get; protected internal set; }
    }
}