namespace Tripod.Domain.Security
{
    public class UserClaim : EntityWithId<int>
    {
        protected internal UserClaim() { }

        public int UserId { get; protected internal set; }
        public virtual User User { get; protected internal set; }

        public string ClaimType { get; protected internal set; }
        public string ClaimValue { get; protected internal set; }
    }
}
