using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class PermissionTests
    {
        [Fact]
        public void InternalCtor_SetsNameProperty()
        {
            const string name = "Permission Name";
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
