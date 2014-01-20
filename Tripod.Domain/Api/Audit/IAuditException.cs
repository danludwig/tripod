using System;

namespace Tripod
{
    public interface IAuditException
    {
        void Audit(Exception exception);
    }
}
