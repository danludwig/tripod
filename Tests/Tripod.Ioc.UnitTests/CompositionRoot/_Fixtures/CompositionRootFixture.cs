using SimpleInjector;

namespace Tripod.Ioc
{
    public class CompositionRootFixture
    {
        public Container Container { get; private set; }

        public CompositionRootFixture()
        {
            Container = new Container();
            Container.ComposeRoot(new RootCompositionSettings());
        }
    }
}