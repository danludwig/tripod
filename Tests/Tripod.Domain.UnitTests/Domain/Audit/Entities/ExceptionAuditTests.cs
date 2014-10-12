using Should;
using Xunit;

namespace Tripod.Domain.Audit
{
    public class ExceptionAuditTests
    {
        [Fact]
        public void NoArgCtor_IsProtected()
        {
            var entity = new ProxiedExceptionAudit();
            entity.ShouldNotBeNull();
        }
    }
}
