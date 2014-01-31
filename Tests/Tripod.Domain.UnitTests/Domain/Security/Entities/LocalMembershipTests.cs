using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class LocalMembershipTests
    {
        [Fact]
        public void Ctor_InitializesNothing()
        {
            var entity = new LocalMembership();
            entity.Id.ShouldEqual(0);
            entity.Owner.ShouldBeNull();
            entity.PasswordHash.ShouldBeNull();
        }
    }
}
