using System;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod
{
    public class EntityWithIdTests
    {
        [Theory, InlineData("testStringId"), InlineData(null)]
        public void GetHashCode_ReturnsHashCodeOStringfId_OrZeroWhenIdIsNull(string id)
        {
            var entity = new FakeEntityWithStringId(id);
            var hashCode = entity.GetHashCode();
            hashCode.ShouldEqual(!Equals(entity.Id, default(string)) ? entity.Id.GetHashCode() : 0);
        }

        [Fact]
        public void EqualsOtherObject_ReturnsFalse_WhenOtherIsNull()
        {
            var entity = new FakeEntityWithStringId("test");

            entity.Equals((object)null).ShouldBeFalse();
        }

        [Fact]
        public void EqualsOtherEntity_ReturnsFalse_WhenOtherIsNull()
        {
            var entity = new FakeEntityWithStringId("test");

            entity.Equals((Entity)null).ShouldBeFalse();
        }

        [Fact]
        public void EqualsOther_ReturnsFalse_WhenOtherIsNull()
        {
            var entity = new FakeEntityWithStringId("test");

            entity.Equals(null).ShouldBeFalse();
        }

        [Fact]
        public void EqualsOther_ReturnsTrue_WhenOtherIsSameReference()
        {
            var entity = new FakeEntityWithStringId("test");

            entity.Equals(entity).ShouldBeTrue();
        }

        [Fact]
        public void EqualsOther_ReturnsFalse_WhenEntityIsTransient()
        {
            var entity = new FakeEntityWithIntId(0);
            var other = new FakeEntityWithIntId(6);

            entity.Equals(other).ShouldBeFalse();
        }

        [Fact]
        public void EqualsOther_ReturnsFalse_WhenOtherIsTransient()
        {
            var entity = new FakeEntityWithIntId(8);
            var other = new FakeEntityWithIntId(0);

            entity.Equals(other).ShouldBeFalse();
        }

        [Fact]
        public void EqualsOther_ReturnsFalse_WhenIdsAreNotEqual()
        {
            var entity = new FakeEntityWithIntId(8);
            var other = new FakeEntityWithIntId(6);

            entity.Equals(other).ShouldBeFalse();
        }

        [Fact]
        public void EqualsOther_ReturnsTrue_WhenIdsAreEqual()
        {
            var id = Guid.NewGuid();
            var entity = new FakeEntityWithGuidId(id);
            var other = new FakeEntityWithGuidId(id);

            entity.Equals(other).ShouldBeTrue();
        }

        [Fact]
        public void EqualsOther_ReturnsTrue_WhenIdsAreEqual_AndEntityIsProxied()
        {
            const long id = long.MaxValue - 4;
            var entity = new FakeProxiedEntityWithLongId(id);
            var other = new FakeEntityWithLongId(id);

            entity.Equals(other).ShouldBeTrue();
        }

        [Fact]
        public void EqualsOther_ReturnsTrue_WhenIdsAreEqual_AndOtherIsProxied()
        {
            const long id = long.MaxValue - 4;
            var entity = new FakeEntityWithLongId(id);
            var other = new FakeProxiedEntityWithLongId(id);

            entity.Equals(other).ShouldBeTrue();
        }
    }
}
