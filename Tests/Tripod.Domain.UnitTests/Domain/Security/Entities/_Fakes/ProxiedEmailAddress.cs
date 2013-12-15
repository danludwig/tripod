namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedEmailAddress : EmailAddress
    {
        public override User Owner { get; protected internal set; }
    }
}