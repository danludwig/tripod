using SimpleInjector;

namespace Tripod.Ioc.Cryptography
{
    public static class CompositionRoot
    {
        public static void RegisterCryptography(this Container container)
        {
            container.RegisterSingle<ICreateSecrets, RngCryptoSecretCreator>();
        }
    }
}
