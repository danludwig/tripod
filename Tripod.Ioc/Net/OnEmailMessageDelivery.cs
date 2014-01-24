using System;
using Tripod.Domain.Security;

namespace Tripod.Ioc.Net
{
    [UsedImplicitly]
    public class OnEmailMessageDelivery : IDeliveredEmailMessage
    {
        private readonly IWriteEntities _entities;

        public OnEmailMessageDelivery(IWriteEntities entities)
        {
            _entities = entities;
        }

        public void OnDelivered(int emailMessageId, Exception error, bool cancelled)
        {
            var entity = _entities.Get<EmailMessage>(emailMessageId);
            entity.LastSendError = error != null ? error.Message : null;
            entity.CancelledOnUtc = cancelled ? DateTime.UtcNow : (DateTime?)null;

            if (error == null && !cancelled)
                entity.SentOnUtc = DateTime.UtcNow;

            _entities.SaveChanges();
        }
    }
}
