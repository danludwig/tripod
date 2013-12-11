using SimpleInjector;
using Xunit;

namespace Tripod.Ioc
{
    public abstract class SimpleInjectorContainerTests : IUseFixture<CompositionRootFixture>
    {
        protected Container Container { get; private set; }

        void IUseFixture<CompositionRootFixture>.SetFixture(CompositionRootFixture fixture)
        {
            Container = fixture.Container;
        }
    }
}