using System.Collections.Specialized;
using System.Net.Mail;
using Moq;
using Xunit;

namespace Tripod.Services.Net
{
    public class InterceptMailDeliveryDecoratorTests
    {
        [Fact]
        public void Test()
        {
            var decorated = new Mock<IDeliverMailMessage>(MockBehavior.Strict);
            decorated.Setup(x => x.Deliver(It.IsAny<MailMessage>(), null, null));
            var configReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            configReader.SetupGet(x => x.AppSettings).Returns(new NameValueCollection
            {
                { AppSettingKey.MailInterceptors.ToString(), string.Format("{0};{1} <{2}>",
                    FakeData.Email(), FakeData.String(), FakeData.Email()) },
            });
            var appConfig = new AppConfiguration(configReader.Object);
            var decorator = new InterceptMailDeliveryDecorator(decorated.Object, appConfig);
            var mailMessage = new MailMessage(new MailAddress(FakeData.Email(), FakeData.String()),
                new MailAddress(FakeData.Email(), FakeData.String()));
            mailMessage.CC.Add(new MailAddress(FakeData.Email(), FakeData.String()));
            mailMessage.Bcc.Add(new MailAddress(FakeData.Email()));

            decorator.Deliver(mailMessage);

            decorated.Verify(x => x.Deliver(It.IsAny<MailMessage>(), null, null), Times.Once);
        }
    }
}
