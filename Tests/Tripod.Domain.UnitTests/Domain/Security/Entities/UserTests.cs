using Microsoft.AspNet.Identity;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class UserTests
    {
        [Fact]
        public void Ctor_InitializesCollections()
        {
            var entity = new User();
            entity.Permissions.ShouldNotBeNull();
            entity.RemoteMemberships.ShouldNotBeNull();
            entity.Claims.ShouldNotBeNull();
            entity.EmailAddresses.ShouldNotBeNull();
        }

        [Fact]
        public void Implements_IUser()
        {
            var user = new User() as IUser<int>;
            user.ShouldNotBeNull();
            user.UserName = "test";
            user.UserName.ShouldEqual("test");
        }
    }
}
