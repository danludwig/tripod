using System;
using Should;
using Xunit;

namespace Tripod
{
    public class ReflectionExtensionTests
    {
        [Fact]
        public void IsGenericallyAssignableFrom_ReturnsTrue_WhenClosedExtendsDirectlyFromOpen()
        {
            var openGeneric = typeof(FakeOpenGenericA<>);
            var closedGeneric = typeof(FakeClosedGenericA1);

            openGeneric.IsGenericallyAssignableFrom(closedGeneric).ShouldBeTrue();
        }

        [Fact]
        public void IsGenericallyAssignableFrom_ReturnsTrue_WhenClosedExtendsIndirectlyFromOpen()
        {
            var openGeneric = typeof(FakeOpenGenericA<>);
            var closedGeneric = typeof(FakeClosedGenericA2);

            openGeneric.IsGenericallyAssignableFrom(closedGeneric).ShouldBeTrue();
        }

        [Fact]
        public void IsGenericallyAssignableFrom_ReturnsTrue_WhenClosedImplementsOpenInterface()
        {
            var openGeneric = typeof(IFakeOpenGenericB<>);
            var closedGeneric = typeof(FakeClosedGenericB1);

            openGeneric.IsGenericallyAssignableFrom(closedGeneric).ShouldBeTrue();
        }

        [Fact]
        public void IsGenericallyAssignableFrom_ReturnsFalse_WhenClosedHasNoBaseType()
        {
            var openGeneric = typeof(FakeOpenGenericA<>);
            var closedGeneric = typeof(object);

            openGeneric.IsGenericallyAssignableFrom(closedGeneric).ShouldBeFalse();
        }

        [Fact]
        public void IsGenericallyAssignableFrom_ReturnsFalse_WhenClosedDoesNotExtendOpen()
        {
            var openGeneric = typeof(FakeOpenGenericA<>);
            var closedGeneric = typeof(FakeClosedGenericB1);

            openGeneric.IsGenericallyAssignableFrom(closedGeneric).ShouldBeFalse();
        }

        [Fact]
        public void PropertyName_ThrowsArgumentNullException_WhenThisArgIsNull()
        {
            FakeStringLengthCommand owner = null;
            // ReSharper disable ExpressionIsAlwaysNull
            var exception = Assert.Throws<ArgumentNullException>(() => owner.PropertyName(x => x.StringProperty));
            // ReSharper restore ExpressionIsAlwaysNull
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("owner");
        }

        [Fact]
        public void PropertyName_NotSupportedException_WhenExpressionIsNotProperty()
        {
            var owner = new FakeOuterObject();
            var exception = Assert.Throws<NotSupportedException>(() =>
                owner.PropertyName(x => x.Method()));
            exception.ShouldNotBeNull();
        }

        [Fact]
        public void PropertyName_ReturnsPropertyName()
        {
            var owner = new FakeOuterObject();
            var propertyName = owner.PropertyName(x => x.Inner);
            propertyName.ShouldEqual("Inner");
        }

        [Fact]
        public void PropertyName_ReturnsDotSeparatedName_ForNestedProperty_ByDefault()
        {
            var owner = new FakeOuterObject();
            var propertyName = owner.PropertyName(x => x.Inner.Property);
            propertyName.ShouldEqual("Inner.Property");
        }

        [Fact]
        public void PropertyName_ReturnsNestedPropertyNameOnly_ForNestedProperty_WhenFullNameIsNotRequested()
        {
            var owner = new FakeOuterObject();
            var propertyName = owner.PropertyName(x => x.Inner.Property, false);
            propertyName.ShouldEqual("Property");
        }
    }
}
