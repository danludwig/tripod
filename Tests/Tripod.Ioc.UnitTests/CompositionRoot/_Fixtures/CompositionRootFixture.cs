using System.Reflection;
using SimpleInjector;

namespace Tripod.Ioc
{
    public class CompositionRootFixture
    {
        public Container Container { get; private set; }

        public CompositionRootFixture()
        {
            Container = new Container();
            var assemblies = new[] { Assembly.GetExecutingAssembly() };
            var settings = new RootCompositionSettings
            {
                FluentValidatorAssemblies = assemblies,
                QueryHandlerAssemblies = assemblies,
                CommandHandlerAssemblies = assemblies,
            };
            Container.ComposeRoot(settings);
        }
    }
}