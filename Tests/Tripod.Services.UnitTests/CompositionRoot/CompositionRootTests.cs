using System;
using System.Linq;
using Should;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using Xunit;

namespace Tripod.Services
{
    public class CompositionRootTests : SimpleInjectorContainerTests
    {
        [Fact]
        public void ComposeRoot_ComposesVerifiedRoot_WithoutDiagnosticsWarnings()
        {
            Container.Verify();
            var results = Analyzer.Analyze(Container);
            results.Any().ShouldBeFalse();
        }

        [Fact]
        public void ComposeRoot_AllowsOverridingRegistrations()
        {
            Container.Options.AllowOverridingRegistrations.ShouldBeTrue();
        }

        [Fact]
        public void ComposeRoot_UsesDefaultSettings_WhenNoneArePassed()
        {
            var container = new Container();
            container.ComposeRoot(null);
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
