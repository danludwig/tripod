using System;
using System.ComponentModel;
using System.Net.Mail;
using Tripod.Domain.Security;

namespace Tripod.Ioc.Net
{
    [UsedImplicitly]
    public class ActiveEmailMessageDelivery : IDeliverEmailMessage
    {
        private readonly IWriteEntities _entities;
        private readonly IDeliverMailMessage _mail;
        private readonly IDeliveredEmailMessage _email;

        public ActiveEmailMessageDelivery(IWriteEntities entities, IDeliverMailMessage mail, IDeliveredEmailMessage email)
        {
            _entities = entities;
            _mail = mail;
            _email = email;
        }

        public void Deliver(int emailMessageId)
        {
            var entity = _entities.Query<EmailMessage>()
                .EagerLoad(x => x.Owner)
                .ById(emailMessageId, false)
            ;

            // don't send the message if it has already been sent
            if (entity.SentOnUtc.HasValue) return;

            // don't sent the message if it is not supposed to be sent yet
            if (entity.SendOnUtc > DateTime.UtcNow) return;

            var from = new MailAddress(entity.From);
            var to = new MailAddress(entity.Owner.Value);
            var mailMessage = new MailMessage(from, to)
            {
                Subject = entity.Subject,
                Body = entity.Body,
                IsBodyHtml = entity.IsBodyHtml,
            };

            var sendState = new SendEmailMessageState
            {
                EmailMessageId = emailMessageId,
            };
            _mail.Deliver(mailMessage, OnSendCompleted, sendState);
        }

        private class SendEmailMessageState
        {
            public int EmailMessageId { get; set; }
        }

        private void OnSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var state = (SendEmailMessageState) e.UserState;
            _email.OnDelivered(state.EmailMessageId, e.Error, e.Cancelled);
        }
    }
}
