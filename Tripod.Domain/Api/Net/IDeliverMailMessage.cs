using System.Net.Mail;

namespace Tripod
{
    public interface IDeliverMailMessage
    {
        void Deliver(MailMessage mailMessage, SendCompletedEventHandler sendCompleted = null, object userState = null);
    }
}
