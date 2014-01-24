using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class UserClaimTests
    {
        [Fact]
        public void Ctor_InitializesNothing()
        {
            var entity = new UserClaim();
            entity.Id.ShouldEqual(0);
            entity.Owner.ShouldBeNull();
            entity.UserId.ShouldEqual(0);
            entity.ClaimType.ShouldBeNull();
            entity.ClaimValue.ShouldBeNull();
        }
    }
}
