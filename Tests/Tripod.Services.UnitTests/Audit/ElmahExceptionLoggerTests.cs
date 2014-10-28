using System;
using System.Collections.Specialized;
using System.Net.Mail;
using Moq;
using Xunit;

namespace Tripod.Services.Audit
{
    public class ElmahExceptionLoggerTests
    {
        [Fact]
        public void Audit_DeliversMailMessage()
        {
            var delivery = new Mock<IDeliverMailMessage>(MockBehavior.Strict);
            delivery.Setup(x => x.Deliver(It.IsAny<MailMessage>(), null, null));
            var configReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            configReader.Setup(x => x.AppSettings).Returns(new NameValueCollection
            {
                { AppSettingKey.MailExceptionTo.ToString(),
                    string.Format("{0};{1}", FakeData.Email(), FakeData.Email()) },
            });
            var appConfig = new AppConfiguration(configReader.Object);
            var auditor = new ElmahExceptionAuditor(delivery.Object, appConfig);

            auditor.Audit(new InvalidOperationException("This app is farting."));

            delivery.Verify(x => x.Deliver(It.IsAny<MailMessage>(), null, null), Times.Once);
        }
    }
}
