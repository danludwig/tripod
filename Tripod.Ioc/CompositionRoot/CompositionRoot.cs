using System;
using SimpleInjector;
using Tripod.Ioc.EntityFramework;

namespace Tripod.Ioc
{
    public static class CompositionRoot
    {
        public static void ComposeRoot(this Container container, RootCompositionSettings settings)
        {
#if !DEBUG
            settings.IsGreenfield = false;
#endif
            container.Options.AllowOverridingRegistrations = true;
            container.Register<IServiceProvider>(() => container, Lifestyle.Singleton);
            container.RegisterEntityFramework(settings);
        }
    }
}
