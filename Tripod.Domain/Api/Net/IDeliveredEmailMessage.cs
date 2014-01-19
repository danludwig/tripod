using System;

namespace Tripod
{
    public interface IDeliveredEmailMessage
    {
        void OnDelivered(int emailMessageId, Exception error, bool cancelled);
    }
}