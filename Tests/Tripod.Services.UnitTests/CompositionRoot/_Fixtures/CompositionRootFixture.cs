using System.Reflection;
using System.Web;
using SimpleInjector;

namespace Tripod.Services
{
    public class CompositionRootFixture
    {
        public Container Container { get; private set; }

        public CompositionRootFixture()
        {
            HttpContext.Current = null;
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