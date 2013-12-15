namespace Tripod.Domain.Security
{
    public class Claim : EntityWithId<int>
    {
        protected internal Claim() { }

        public int UserId { get; protected internal set; }
        public virtual User Owner { get; protected internal set; }

        public string ClaimType { get; protected internal set; }
        public string ClaimValue { get; protected internal set; }
    }
}
