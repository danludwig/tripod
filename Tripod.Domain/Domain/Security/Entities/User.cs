using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class User : EntityWithId<int>, IUser<int>
    {
        protected internal User()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Permissions = new List<Permission>();
            RemoteMemberships = new List<RemoteMembership>();
            Claims = new List<UserClaim>();
            EmailAddresses = new List<EmailAddress>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public string Name { get; protected internal set; }

        string IUser<int>.UserName
        {
            get { return Name; }
            set { Name = value; }
        }

        public virtual ICollection<Permission> Permissions { get; private set; }

        public virtual LocalMembership LocalMembership { get; protected internal set; }

        public virtual ICollection<RemoteMembership> RemoteMemberships { get; private set; }

        public virtual ICollection<UserClaim> Claims { get; private set; }

        public virtual ICollection<EmailAddress> EmailAddresses { get; private set; }

        public string SecurityStamp { get; protected internal set; }

        public static class Constraints
        {
            public const string Label = "User";

            public const string NameLabel = "User name";
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;
            public const int SecurityStampMaxLength = 250;
            public const string AllowedNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
        }
    }
}
