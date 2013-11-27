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

            openGeneric.IsGenericallyAssignableFrom(closedGeneric).ShouldEqual(true);
        }

        [Fact]
        public void IsGenericallyAssignableFrom_ReturnsTrue_WhenClosedExtendsIndirectlyFromOpen()
        {
            var openGeneric = typeof(FakeOpenGenericA<>);
            var closedGeneric = typeof(FakeClosedGenericA2);

            openGeneric.IsGenericallyAssignableFrom(closedGeneric).ShouldEqual(true);
        }

        [Fact]
        public void IsGenericallyAssignableFrom_ReturnsTrue_WhenClosedImplementsOpenInterface()
        {
            var openGeneric = typeof(IFakeOpenGenericB<>);
            var closedGeneric = typeof(FakeClosedGenericB1);

            openGeneric.IsGenericallyAssignableFrom(closedGeneric).ShouldEqual(true);
        }

        [Fact]
        public void IsGenericallyAssignableFrom_ReturnsFalse_WhenClosedHasNoBaseType()
        {
            var openGeneric = typeof(FakeOpenGenericA<>);
            var closedGeneric = typeof(object);

            openGeneric.IsGenericallyAssignableFrom(closedGeneric).ShouldEqual(false);
        }


        [Fact]
        public void IsGenericallyAssignableFrom_ReturnsFalse_WhenClosedDoesNotExtendOpen()
        {
            var openGeneric = typeof(FakeOpenGenericA<>);
            var closedGeneric = typeof(FakeClosedGenericB1);

            openGeneric.IsGenericallyAssignableFrom(closedGeneric).ShouldEqual(false);
        }
    }
}
