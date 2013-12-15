using System.Collections.Generic;
using System.Linq;

namespace Tripod.Domain.Security
{
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
    }
}