using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tripod.Domain.Security
{
    public class Permission : EntityWithId<int>
    {
        protected Permission()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Users = new Collection<User>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        internal Permission(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }

        public string Description { get; protected internal set; }

        public virtual ICollection<User> Users { get; protected set; }

        public static class Constraints
        {
            public const string Label = "Permission";

            public const string NameLabel = "Permission name";
            public const int NameMaxLength = 256;

            public const string DescriptionLabel = "Permission description";
            public const int DescriptionMaxLength = 4000;
        }
    }
}
