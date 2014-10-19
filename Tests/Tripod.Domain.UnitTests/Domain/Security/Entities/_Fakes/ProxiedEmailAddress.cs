namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedEmailAddress : EmailAddress
    {
        protected internal ProxiedEmailAddress() { }

        protected internal ProxiedEmailAddress(int id) : this()
        {
            Id = id;
        }

        public override User User { get; protected internal set; }
    }
}