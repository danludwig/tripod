namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedClaim : UserClaim
    {
        public override User User { get; protected internal set; }
    }
}