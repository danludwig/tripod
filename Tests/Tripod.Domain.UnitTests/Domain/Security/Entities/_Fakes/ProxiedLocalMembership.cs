namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedLocalMembership : LocalMembership
    {
        public override User Owner { get; protected internal set; }
    }
}