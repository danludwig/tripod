using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class RemoteMembershipTests
    {
        [Fact]
        public void Ctor_CreatesEmptyId()
        {
            var entity = new RemoteMembership();
            entity.Id.ShouldNotBeNull();
        }

        [Fact]
        public void ComponentProperties_DelegateTo_CompositeId()
        {
            var entity = new RemoteMembership
            {
                Id =
                {
                    LoginProvider = "provider1",
                    ProviderKey = "key1"
                }
            };
            entity.LoginProvider.ShouldEqual(entity.Id.LoginProvider);
            entity.ProviderKey.ShouldEqual(entity.Id.ProviderKey);
            entity = new ProxiedRemoteMembership("provider2", "key2");
            entity.LoginProvider.ShouldEqual(entity.Id.LoginProvider);
            entity.ProviderKey.ShouldEqual(entity.Id.ProviderKey);
        }

        [Theory, InlineData(null, null), InlineData("provider", null), InlineData(null, "key")]
        public void CompositeId_GetHashCode_ReturnsZero_WhenAnyComponentIsNull(string loginProvider, string providerKey)
        {
            var id = new RemoteMembershipId
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey,
            };
            id.GetHashCode().ShouldEqual(0);
        }

        [Fact]
        public void CompositeId_GetHashCode_IsBasedOnAllComponents()
        {
            var id = new RemoteMembershipId
            {
                LoginProvider = "loginProvider",
                ProviderKey = "providerKey",
            };
            id.GetHashCode().ShouldEqual(id.LoginProvider.GetHashCode() ^ id.ProviderKey.GetHashCode());
        }

        [Fact]
        public void CompositeId_Equals_ReturnsFalse_WhenOtherIsNull()
        {
            var id = new RemoteMembershipId
            {
                LoginProvider = "loginProvider",
                ProviderKey = "providerKey",
            };
            id.Equals(null).ShouldBeFalse();
            id.Equals(null as object).ShouldBeFalse();
        }

        [Fact]
        public void CompositeId_Equals_ReturnsTrue_WhenReferenceEqualsOther()
        {
            var id = new RemoteMembershipId
            {
                LoginProvider = "loginProvider",
                ProviderKey = "providerKey",
            };
            id.Equals(id).ShouldBeTrue();
            id.Equals(id as object).ShouldBeTrue();
        }

        [Theory]
        [InlineData(null, "key1", "provider2", "key2")]
        [InlineData("provider1", null, "provider2", "key2")]
        [InlineData("provider1", "key1", null, "key2")]
        [InlineData("provider1", "key1", "provider2", null)]
        public void CompositeId_Equals_ReturnsFalse_WhenAnyComponentIsNull(string loginProvider1, string providerKey1, string loginProvider2, string providerKey2)
        {
            var id1 = new RemoteMembershipId
            {
                LoginProvider = loginProvider1,
                ProviderKey = providerKey1,
            };
            var id2 = new RemoteMembershipId
            {
                LoginProvider = loginProvider2,
                ProviderKey = providerKey2,
            };
            id1.Equals(id2).ShouldBeFalse();
            id1.Equals(id2 as object).ShouldBeFalse();
            id2.Equals(id1).ShouldBeFalse();
            id2.Equals(id1 as object).ShouldBeFalse();
        }

        [Theory]
        [InlineData("provider1", "key1", "provider2", "key1")]
        [InlineData("provider1", "key1", "provider1", "key2")]
        public void CompositeId_Equals_ReturnsFalse_WhenAnyComponent_IsUnequal(string loginProvider1, string providerKey1, string loginProvider2, string providerKey2)
        {
            var id1 = new RemoteMembershipId
            {
                LoginProvider = loginProvider1,
                ProviderKey = providerKey1,
            };
            var id2 = new RemoteMembershipId
            {
                LoginProvider = loginProvider2,
                ProviderKey = providerKey2,
            };
            id1.Equals(id2).ShouldBeFalse();
            id1.Equals(id2 as object).ShouldBeFalse();
            id2.Equals(id1).ShouldBeFalse();
            id2.Equals(id1 as object).ShouldBeFalse();
        }

        [Theory]
        [InlineData("provider1", "key1", "provider1", "key1")]
        [InlineData("provider2", "key2", "provider2", "key2")]
        public void CompositeId_Equals_ReturnsTrue_WhenAllComponents_AreEqual(string loginProvider1, string providerKey1, string loginProvider2, string providerKey2)
        {
            var id1 = new RemoteMembershipId
            {
                LoginProvider = loginProvider1,
                ProviderKey = providerKey1,
            };
            var id2 = new RemoteMembershipId
            {
                LoginProvider = loginProvider2,
                ProviderKey = providerKey2,
            };
            id1.Equals(id2).ShouldBeTrue();
            id1.Equals(id2 as object).ShouldBeTrue();
            id2.Equals(id1).ShouldBeTrue();
            id2.Equals(id1 as object).ShouldBeTrue();
        }
    }
}
