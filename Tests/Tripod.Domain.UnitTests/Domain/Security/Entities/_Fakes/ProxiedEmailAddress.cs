namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedEmailAddress : EmailAddress
    {
        public override User User { get; protected internal set; }
    }
}