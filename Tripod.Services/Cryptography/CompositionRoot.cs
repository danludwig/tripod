using Microsoft.Owin.Security.DataProtection;
using SimpleInjector;

namespace Tripod.Services.Cryptography
{
    public static class CompositionRoot
    {
        public static void RegisterCryptography(this Container container)
        {
            container.RegisterSingle<ICreateSecrets, RngCryptoSecretCreator>();

            // note that changing the appname argument below will invalidate any previously generated tokens
            container.Register<IDataProtectionProvider>(() => new DpapiDataProtectionProvider(AppConfiguration.DataProtectionAppName));
        }
    }
}
