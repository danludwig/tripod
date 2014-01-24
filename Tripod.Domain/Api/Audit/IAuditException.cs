using System;

namespace Tripod
{
    public interface IAuditException
    {
        [UsedImplicitly]
        void Audit(Exception exception);
    }
}
