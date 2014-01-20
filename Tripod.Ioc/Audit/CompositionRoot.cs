using SimpleInjector;

namespace Tripod.Ioc.Audit
{
    public static class CompositionRoot
    {
        public static void RegisterExceptionAuditor(this Container container)
        {
            container.Register<IAuditException, ElmahExceptionAuditor>();
        }
    }
}
