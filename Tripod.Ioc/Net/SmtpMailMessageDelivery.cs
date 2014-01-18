using System;
using System.Net.Mail;

namespace Tripod.Ioc.Net
{
    public class SmtpMailMessageDelivery : IDeliverMailMessage, IDisposable
    {
        public SmtpMailMessageDelivery()
        {
            SmtpClient = new SmtpClient();
        }

        public void Dispose()
        {
            SmtpClient.Dispose();
        }

        protected SmtpClient SmtpClient { get; private set; }

        public virtual void Deliver(MailMessage message)
        {
            SmtpClient.Send(message);
        }
    }
}
