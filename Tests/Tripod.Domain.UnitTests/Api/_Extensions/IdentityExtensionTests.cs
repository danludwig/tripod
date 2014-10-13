using System;
using System.Security.Claims;
using System.Security.Principal;
using Should;
using Xunit;

namespace Tripod
{
    public class IdentityExtensionTests
    {
        [Fact]
        public void HasUserId_ReturnsTrue_WhenClaimsIdentity_HasNameIdentifier()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "value"),
            });
            var result = identity.HasAppUserId();
            result.ShouldBeTrue();
        }

        [Fact]
        public void HasUserId_ReturnsFalse_WhenClaimsIdentity_HasNoNameIdentifier()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Country, "value"),
            });
            var result = identity.HasAppUserId();
            result.ShouldBeFalse();
        }

        [Fact]
        public void HasUserId_ReturnsFalse_WhenIdentity_IsNotClaimsIdentity()
        {
            var identity = new GenericIdentity("username");
            var result = identity.HasAppUserId();
            result.ShouldBeFalse();
        }

        [Fact]
        public void HasUserId_ThrowsArgumentNullException_WhenIdentityIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => (null as IIdentity).HasAppUserId());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("identity");
        }
    }
}
