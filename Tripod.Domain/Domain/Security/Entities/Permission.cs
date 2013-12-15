using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class Permission : EntityWithId<int>, IRole<int>
    {
        protected internal Permission()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Users = new List<User>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public string Name { get; protected internal set; }

        string IRole<int>.Name { get; set; }

        public string Description { get; protected internal set; }

        public virtual ICollection<User> Users { get; private set; }

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
