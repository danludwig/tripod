using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class UserTests
    {
        [Fact]
        public void InternalCtor_SetsNameProperty()
        {
            const string userName = "username";
            var entity = new User { Name = userName };
            entity.Name.ShouldEqual(userName);
        }

        [Fact]
        public void NoArgCtor_IsProtected()
        {
            var entity = new ProxiedUser();
            entity.ShouldNotBeNull();
        }
    }
}
