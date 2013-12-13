using System;
using System.Linq;
using Should;
using SimpleInjector;
using Xunit;

namespace Tripod.Ioc
{
    public class WebApiDependencyResolverTests : SimpleInjectorContainerTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenContainerArgIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new WebApiDependencyResolver(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("container");
        }

        [Fact]
        public void GetService_ReturnsInstanceOfService()
        {
            var resolver = new WebApiDependencyResolver(Container);
            var service = resolver.GetService(typeof(IServiceProvider));
            service.ShouldEqual(Container);
        }

        [Fact]
        public void GetServices_ReturnsInstancesOfService()
        {
            var container = new Container();
            container.RegisterAll<IFakeMultipleServices>(typeof(FakeMultipleService1), typeof(FakeMultipleService2), typeof(FakeMultipleService3));
            container.Verify();
            var resolver = new WebApiDependencyResolver(container);
            var services = resolver.GetServices(typeof(IFakeMultipleServices)).ToArray();
            services.ShouldNotBeNull();
            services.Length.ShouldEqual(3);
            var serviceTypes = services.Select(x => x.GetType()).ToArray();
            serviceTypes.ShouldContain(typeof(FakeMultipleService1));
            serviceTypes.ShouldContain(typeof(FakeMultipleService2));
            serviceTypes.ShouldContain(typeof(FakeMultipleService3));
        }

        [Fact]
        public void BeginScope_ReturnsSameInstanceOfResolver()
        {
            var resolver = new WebApiDependencyResolver(Container);
            var scope = resolver.BeginScope();
            scope.ShouldEqual(resolver);
        }

        [Fact]
        public void Dispose_IsImplemented_AsNoOp()
        {
            var resolver = new WebApiDependencyResolver(Container);
            resolver.Dispose();
            resolver.ShouldNotBeNull();
        }
    }
}
