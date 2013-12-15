namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedClaim : Claim
    {
        public override User Owner { get; protected internal set; }
    }
}