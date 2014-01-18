using Tripod.Domain.Security;

namespace Tripod
{
    public interface IDeliverEmailMessage
    {
        void Deliver(EmailMessage emailMessage);
    }
}
