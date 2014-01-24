using System.ComponentModel;
using System.Net.Mail;
using System.Threading;
using System.Web;
using Elmah;

namespace Tripod.Ioc.Net
{
    public class RetryDeliverMailDecorator : IDeliverMailMessage
    {
        private readonly IDeliverMailMessage _decorated;

        public RetryDeliverMailDecorator(IDeliverMailMessage decorated)
        {
            _decorated = decorated;
        }

        public void Deliver(MailMessage message, SendCompletedEventHandler sendCompleted = null, object userState = null)
        {
            _decorated.Deliver(message, GetOnSendCompleted(message, sendCompleted), new RetryUserState
            {
                UserState = userState,
                CountDown = 3,
            });
        }

        private class RetryUserState
        {
            public object UserState { get; set; }
            public short CountDown { get; set; }
        }

        private SendCompletedEventHandler GetOnSendCompleted(MailMessage message, SendCompletedEventHandler sendCompleted)
        {
            SendCompletedEventHandler handler = (sender, e) =>
            {
                var retryUserState = (RetryUserState)e.UserState;
                var countDown = retryUserState.CountDown;
                if (e.Error != null && --countDown > 0)
                {
                    Thread.Sleep(3000);
                    retryUserState.CountDown = countDown;
                    _decorated.Deliver(message, null, retryUserState);
                }
                else if (sendCompleted != null)
                {
                    if (e.Error != null)
                    {
                        var error = new Error(e.Error);
                        var log = ErrorLog.GetDefault(HttpContext.Current);
                        log.Log(error);
                    }

                    sendCompleted(sender, new AsyncCompletedEventArgs(e.Error, e.Cancelled, retryUserState.UserState));
                }
            };
            return handler;
        }
    }
}