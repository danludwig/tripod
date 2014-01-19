using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Tripod.Ioc.Net
{
    public class SmtpMailMessageDelivery : IDeliverMailMessage, IDisposable
    {
        public SmtpMailMessageDelivery()
        {
            SmtpClientInstance = new SmtpClient();
        }

        public void Dispose()
        {
            SmtpClientInstance.Dispose();
        }

        protected SmtpClient SmtpClientInstance { get; private set; }

        public virtual void Deliver(MailMessage message, SendCompletedEventHandler sendCompleted = null, object userState = null)
        {
            if (sendCompleted != null) SmtpClientInstance.SendCompleted += sendCompleted;
            Task.Factory.StartNew(() => SmtpClientInstance.SendAsync(message, userState));
        }
    }
}
