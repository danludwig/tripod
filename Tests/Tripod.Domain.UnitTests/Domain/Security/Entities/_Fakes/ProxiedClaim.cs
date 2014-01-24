namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedClaim : UserClaim
    {
        public override User Owner { get; protected internal set; }
    }
}