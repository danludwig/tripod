using System;
using System.Reflection;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod
{
    public class ReflectionExtensionTests
    {
        #region IsGenericallyAssignableFrom

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

        #endregion
        #region PropertyName

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

        #endregion
        #region GetManifestResourceText

        [Theory]
        [InlineData("Tripod.Api._Extensions._Fakes.FakeResource1.txt", "For Testing")]
        [InlineData("Tripod.Api._Extensions._Fakes.FakeResource1.xml", "<element>\r\n</element>\r\n")]
        public void GetManifestResourceText_ReturnsFullResourceText(string resourceWithText, string expectedText)
        {
            var result = Assembly.GetExecutingAssembly().GetManifestResourceText(resourceWithText);
            result.ShouldEqual(expectedText);
        }

        [Fact]
        public void GetManifestResourceText_ThrowsInvalidOperationException_WhenResourceDoesNotExist()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Assembly.GetExecutingAssembly().GetManifestResourceText("FakeResourceX.ell"));
            exception.ShouldNotBeNull();
        }

        #endregion
        #region GetManifestResourceName

        [Theory]
        [InlineData("FakeResource1.txt")]
        [InlineData("FakeResource1.xml")]
        public void GetManifestResourceName_ReturnsFullResourceName(string resourceToFind)
        {
            var result = Assembly.GetExecutingAssembly().GetManifestResourceName(resourceToFind);
            result.ShouldEqual(string.Format("Tripod.Api._Extensions._Fakes.{0}", resourceToFind));
        }

        [Fact]
        public void GetManifestResourceName_ThrowsInvalidOperationException_WhenResourceDoesNotExist()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Assembly.GetExecutingAssembly().GetManifestResourceName("FakeResourceX.ell"));
            exception.ShouldNotBeNull();
        }

        #endregion
    }
}
