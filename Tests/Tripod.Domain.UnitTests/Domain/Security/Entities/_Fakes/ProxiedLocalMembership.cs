namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedLocalMembership : LocalMembership
    {
        public override User User { get; protected internal set; }
    }
}