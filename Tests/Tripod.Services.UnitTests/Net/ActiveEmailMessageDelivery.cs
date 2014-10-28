using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using Moq;
using Tripod.Domain.Security;
using Xunit;

namespace Tripod.Services.Net
{
    public class ActiveEmailMessageDeliveryTests
    {
        [Fact]
        public void Deliver_DeliversMailMessage()
        {
            var emailMessageId = FakeData.Id();
            EmailMessage emailMessageToSend = new ProxiedEmailMessage(emailMessageId)
            {
                From = FakeData.Email(),
                EmailAddress = new ProxiedEmailAddress(FakeData.Id())
                {
                    Value = FakeData.Email(),
                }
            };
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var messageData = new[]
            {
                new ProxiedEmailMessage(FakeData.Id()),
                emailMessageToSend,
                new ProxiedEmailMessage(FakeData.Id()),
            }.AsQueryable();
            var messageSet = new Mock<DbSet<EmailMessage>>(MockBehavior.Strict)
                .SetupDataAsync(messageData);
            entities.Setup(x => x.Query<EmailMessage>()).Returns(messageSet.Object);
            var deliver = new Mock<IDeliverMailMessage>(MockBehavior.Strict);
            deliver.Setup(x => x.Deliver(It.IsAny<MailMessage>(),
                It.IsAny<SendCompletedEventHandler>(), It.IsAny<object>()));
            var delivered = new Mock<IDeliveredEmailMessage>(MockBehavior.Strict);
            var delivery = new ActiveEmailMessageDelivery(
                entities.Object, deliver.Object, delivered.Object);

            delivery.Deliver(emailMessageId);

            deliver.Verify(x => x.Deliver(It.IsAny<MailMessage>(),
                It.IsAny<SendCompletedEventHandler>(), It.IsAny<object>()), Times.Once);
        }
    }
}
