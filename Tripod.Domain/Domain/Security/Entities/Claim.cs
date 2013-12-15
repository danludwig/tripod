namespace Tripod.Domain.Security
{
    public class Claim : EntityWithId<int>
    {
        protected internal Claim() { }

        public int UserId { get; protected internal set; }
        public virtual User Owner { get; protected set; }

        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }
    }
}
