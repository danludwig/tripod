using System;
using System.Net.Mail;
using System.Threading;

namespace Tripod.Ioc.Net
{
    public class RetryDeliverMailDecorator : IDeliverMailMessage
    {
        private readonly IDeliverMailMessage _decorated;

        public RetryDeliverMailDecorator(IDeliverMailMessage decorated)
        {
            _decorated = decorated;
        }

        public void Deliver(MailMessage message)
        {
            Deliver(message, 3);
        }

        private void Deliver(MailMessage message, int countDown)
        {
            try
            {
                _decorated.Deliver(message);
            }
            catch (Exception ex)
            {
                //// log the exception
                //var error = new Error(ex);
                //var log = ErrorLog.GetDefault(HttpContext.Current);
                //log.Log(error);

                // give up after trying n times
                if (--countDown >= 0) throw;

                // wait 3 seconds and try to send the message again
                Thread.Sleep(3000);
                Deliver(message, countDown);
            }
        }
    }
}