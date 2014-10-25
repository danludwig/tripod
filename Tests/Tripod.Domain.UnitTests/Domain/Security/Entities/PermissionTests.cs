using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class PermissionTests
    {
        [Fact]
        public void InternalCtor_SetsNameProperty()
        {
            var name = FakeData.String();
            var entity = new Permission { Name = name };
            entity.Name.ShouldEqual(name);
        }

        [Fact]
        public void NoArgCtor_IsProtected()
        {
            var entity = new ProxiedPermission();
            entity.ShouldNotBeNull();
        }
    }
}
