using System;
using SimpleInjector;
using Tripod.Ioc.Cryptography;
using Tripod.Ioc.EntityFramework;
using Tripod.Ioc.FluentValidation;
using Tripod.Ioc.Net;
using Tripod.Ioc.Security;
using Tripod.Ioc.Transactions;

namespace Tripod.Ioc
{
    public static class CompositionRoot
    {
        public static void ComposeRoot(this Container container, RootCompositionSettings settings)
        {
            settings = settings ?? new RootCompositionSettings();
#if !DEBUG
            settings.IsGreenfield = false;
#endif
            container.Options.AllowOverridingRegistrations = true;
            container.Register<IServiceProvider>(() => container, Lifestyle.Singleton);
            container.RegisterCryptography();
            container.RegisterMailDelivery();
            container.RegisterEntityFramework(settings.IsGreenfield);
            container.RegisterSecurity();
            container.RegisterFluentValidation(settings.FluentValidatorAssemblies);
            container.RegisterQueryTransactions(settings.QueryHandlerAssemblies);
            container.RegisterCommandTransactions(settings.CommandHandlerAssemblies);
        }
    }
}
