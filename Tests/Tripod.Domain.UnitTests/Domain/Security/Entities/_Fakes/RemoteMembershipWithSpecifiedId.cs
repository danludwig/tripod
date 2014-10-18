namespace Tripod.Domain.Security
{
    public class RemoteMembershipWithSpecifiedId : RemoteMembership
    {
        public RemoteMembershipWithSpecifiedId(RemoteMembershipId id)
        {
            Id = id;
        }
    }
}