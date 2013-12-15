using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class EmailAddressTests
    {
        [Fact]
        public void Ctor_InitializesNothing()
        {
            var entity = new EmailAddress();
            entity.Id.ShouldEqual(0);
            entity.Owner.ShouldBeNull();
            entity.OwnerId.ShouldBeNull();
            entity.Value.ShouldBeNull();
            entity.IsDefault.ShouldBeFalse();
            entity.IsConfirmed.ShouldBeFalse();
        }
    }
}
