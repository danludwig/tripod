using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using Should;
using Xunit;

namespace Tripod
{
    public class AppConfigurationTests
    {
        [Fact]
        public void XsrfKey_Returns_ValueFromConfig()
        {
            string xsrfKey = AppConfiguration.XsrfKey;
            xsrfKey.ShouldEqual(ConfigurationManager.AppSettings[AppSettingKey.XsrfKey.ToString()]);
        }

        [Fact]
        public void MailFromDefault_Returns_MailAddressBasedOnConfig()
        {
            MailAddress mailFromDefault = AppConfiguration.MailFromDefault;
            Assert.NotNull(mailFromDefault);
            mailFromDefault.DisplayName.ShouldEqual("Test Default Mail From Display Name");
            mailFromDefault.Address.ShouldEqual("mail.from.default@example.tld");
        }

        [Fact]
        public void MailInterceptors_Returns_MailAddressesBasedOnConfig()
        {
            IEnumerable<MailAddress> result = AppConfiguration.MailInterceptors;
            Assert.NotNull(result);
            var mailInterceptors = result.ToArray();
            mailInterceptors.Length.ShouldEqual(2);
            mailInterceptors[0].DisplayName.ShouldEqual("Test Mail Interceptor #1 Display Name");
            mailInterceptors[0].Address.ShouldEqual("mail.interceptors.1@example.tld");
            mailInterceptors[1].DisplayName.ShouldEqual("Test Mail Interceptor #2 Display Name");
            mailInterceptors[1].Address.ShouldEqual("mail.interceptors.2@example.tld");
        }

        [Fact]
        public void MailExceptionTo_Returns_MailAddressesBasedOnConfig()
        {
            IEnumerable<MailAddress> result = AppConfiguration.MailExceptionTo;
            Assert.NotNull(result);
            var mailExceptionTo = result.ToArray();
            mailExceptionTo.Length.ShouldEqual(2);
            mailExceptionTo[0].DisplayName.ShouldEqual("Test Mail Exception To #1 Display Name");
            mailExceptionTo[0].Address.ShouldEqual("mail.exception.to.1@example.tld");
            mailExceptionTo[1].DisplayName.ShouldEqual("Test Mail Exception To #2 Display Name");
            mailExceptionTo[1].Address.ShouldEqual("mail.exception.to.2@example.tld");
        }

        [Fact]
        public void MailPickupDirectory_Returns_ValueFromConfig()
        {
            string result = AppConfiguration.MailPickupDirectory;
            result.ShouldEqual(@"App_Data\mail\pickup");
        }

        [Fact]
        public void MailDeliveryMethod_Returns_ValueFromConfig()
        {
            SmtpDeliveryMethod result = AppConfiguration.MailDeliveryMethod;
            result.ShouldEqual(SmtpDeliveryMethod.Network);
        }
    }
}
