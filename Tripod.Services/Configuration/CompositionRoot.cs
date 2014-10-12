using SimpleInjector;

namespace Tripod.Services.Configuration
{
    public static class CompositionRoot
    {
        public static void RegisterConfiguration(this Container container)
        {
            container.RegisterSingle<IReadConfiguration, ConfigurationManagerReader>();
            container.RegisterSingle<AppConfiguration>();
        }
    }
}
