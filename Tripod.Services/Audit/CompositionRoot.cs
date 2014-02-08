using SimpleInjector;

namespace Tripod.Services.Audit
{
    public static class CompositionRoot
    {
        public static void RegisterExceptionAuditor(this Container container)
        {
            container.Register<IAuditException, ElmahExceptionAuditor>();
        }
    }
}
