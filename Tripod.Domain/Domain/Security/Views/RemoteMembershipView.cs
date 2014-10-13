namespace Tripod.Domain.Security
{
    public class RemoteMembershipView
    {
        public string Provider { get; set; }
        public string Key { get; set; }
        public int? UserId { [UsedImplicitly] get; set; }
    }
}