using System;
using Should;
using SimpleInjector;
using Xunit;

namespace Tripod.Ioc
{
    public class CompositionRootTests : SimpleInjectorContainerTests
    {
        [Fact]
        public void ComposeRoot_ComposesVerifiedRoot()
        {
            Container.Verify();
        }

        [Fact]
        public void ComposeRoot_AllowsOverridingRegistrations()
        {
            Container.Options.AllowOverridingRegistrations.ShouldBeTrue();
        }

        [Fact]
        public void ComposeRoot_RegistersIServiceProvider_UsingOwnContainer_AsSingleton()
        {
            var instance = Container.GetInstance<IServiceProvider>();
            var registration = Container.GetRegistration(typeof (IServiceProvider));

            instance.ShouldNotBeNull();
            instance.ShouldEqual(Container);
            registration.Lifestyle.ShouldEqual(Lifestyle.Singleton);
        }
    }
}
