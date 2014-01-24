using System;

namespace Tripod.Domain.Security
{
    public class RemoteMembership : EntityWithId<RemoteMembershipId>
    {
        protected internal RemoteMembership()
        {
            Id = new RemoteMembershipId();
        }

        public string LoginProvider
        {
            get { return Id.LoginProvider; }
            protected set { Id.LoginProvider = value; }
        }
        public string ProviderKey
        {
            get { return Id.ProviderKey; }
            protected set { Id.ProviderKey = value; }
        }

        public int UserId { get; protected internal set; }
        public virtual User Owner { get; protected internal set; }

        public static class Constraints
        {
            [UsedImplicitly] public const string Label = "OAuth provider";

            public const string ProviderLabel = "OAuth provider name";
            public const int ProviderMaxLength = 130;

            public const string ProviderUserIdLabel = "OAuth provider user id";
            public const int ProviderUserIdMaxLength = 130;
        }
    }

    public class RemoteMembershipId : IEquatable<RemoteMembershipId>
    {
        protected internal RemoteMembershipId() { }
        public string LoginProvider { get; internal set; }
        public string ProviderKey { get; internal set; }

        public override int GetHashCode()
        {
            return IsTransient(this)
                ? 0
                : LoginProvider.GetHashCode() ^ ProviderKey.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as RemoteMembershipId);
        }

        public bool Equals(RemoteMembershipId other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (IsTransient(this) || IsTransient(other)) return false;
            return Equals(LoginProvider, other.LoginProvider) && Equals(ProviderKey, other.ProviderKey);
        }

        private static bool IsTransient(RemoteMembershipId id)
        {
            return id.LoginProvider == null || id.ProviderKey == null;
        }
    }
}
