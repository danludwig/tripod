using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tripod.Domain.Security
{
    public class User : EntityWithId<int>
    {
        protected internal User()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Permissions = new Collection<Permission>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public string Name { get; protected internal set; }

        public virtual ICollection<Permission> Permissions { get; protected internal set; }

        public static class Constraints
        {
            public const string Label = "User";

            public const string NameLabel = "User name";
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;
        }
    }
}
