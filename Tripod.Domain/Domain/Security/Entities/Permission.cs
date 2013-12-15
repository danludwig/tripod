using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class Permission : EntityWithId<int>, IRole<int>
    {
        protected Permission()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Users = new List<User>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        internal Permission(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }

        string IRole<int>.Name { get; set; }

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
