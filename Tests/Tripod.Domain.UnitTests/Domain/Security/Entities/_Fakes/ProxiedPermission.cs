using System.Collections.Generic;

namespace Tripod.Domain.Security
{
    public class ProxiedPermission : Permission
    {
        protected internal ProxiedPermission()
        {
            Name = "nameFromDb";
        }

        public override ICollection<User> Users { get; protected set; }
    }
}