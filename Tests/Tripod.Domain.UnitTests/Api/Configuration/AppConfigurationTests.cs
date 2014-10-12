using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod
{
    public class AppConfigurationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("Test XSRF Key")]
        public void XsrfKey_Returns_ValueFromConfig(string valueInConfig)
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var appSettings = valueInConfig == null
                ? new NameValueCollection()
                : new NameValueCollection
                {
                    { "XsrfKey", valueInConfig },
                }
            ;
            mockConfigReader.SetupGet(x => x.AppSettings).Returns(appSettings);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);
            string result = appConfiguration.XsrfKey;
            result.ShouldEqual(valueInConfig);
        }

        [Theory]
        [InlineData(null, "AppName")]
        [InlineData("Test App name", "Test App name")]
        public void DataProtectionAppName_Returns_ValueFromConfig_Or_AppName(string valueInConfig, string expected)
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var appSettings = valueInConfig == null
                ? new NameValueCollection()
                : new NameValueCollection
                {
                    { "DataProtectionAppName", valueInConfig },
                }
            ;
            mockConfigReader.SetupGet(x => x.AppSettings).Returns(appSettings);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);
            string result = appConfiguration.DataProtectionAppName;
            result.ShouldEqual(expected);
        }

        [Fact]
        public void MailFromDefault_ReturnsMailAddress_BasedOnConfig()
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var appSettings = new NameValueCollection
            {
                {
                    "MailFromDefault",
                    "Test Default Mail From Display Name <mail.from.default@example.tld>"
                },
            };
            mockConfigReader.SetupGet(x => x.AppSettings).Returns(appSettings);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);

            MailAddress mailFromDefault = appConfiguration.MailFromDefault;

            Assert.NotNull(mailFromDefault);
            mailFromDefault.DisplayName.ShouldEqual("Test Default Mail From Display Name");
            mailFromDefault.Address.ShouldEqual("mail.from.default@example.tld");
        }

        [Fact]
        public void MailFromDefault_ReturnsPhonyMailAddress_WhenNoConfigExists()
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var appSettings = new NameValueCollection();
            mockConfigReader.SetupGet(x => x.AppSettings).Returns(appSettings);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);

            MailAddress result = appConfiguration.MailFromDefault;

            Assert.NotNull(result);
            result.DisplayName.ShouldEqual("UNCONFIGURED NOREPLY");
            result.Address.ShouldEqual("no-reply@localhost.tld");
        }

        [Fact]
        public void MailInterceptors_ReturnsMailAddresses_BasedOnConfig()
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var appSettings = new NameValueCollection
            {
                {
                    "MailInterceptors",
                    "Test Mail Interceptor #1 Display Name <mail.interceptors.1@example.tld>;" +
                    "Test Mail Interceptor #2 Display Name <mail.interceptors.2@example.tld>"
                },
            };
            mockConfigReader.SetupGet(x => x.AppSettings).Returns(appSettings);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);

            var result = appConfiguration.MailInterceptors;

            Assert.NotNull(result);
            var mailInterceptors = result.ToArray();
            mailInterceptors.Length.ShouldEqual(2);
            mailInterceptors[0].DisplayName.ShouldEqual("Test Mail Interceptor #1 Display Name");
            mailInterceptors[0].Address.ShouldEqual("mail.interceptors.1@example.tld");
            mailInterceptors[1].DisplayName.ShouldEqual("Test Mail Interceptor #2 Display Name");
            mailInterceptors[1].Address.ShouldEqual("mail.interceptors.2@example.tld");
        }

        [Fact]
        public void MailInterceptors_ReturnsPhonyMailAddresses_WhenNoConfigExists()
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var appSettings = new NameValueCollection();
            mockConfigReader.SetupGet(x => x.AppSettings).Returns(appSettings);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);

            IEnumerable<MailAddress> result = appConfiguration.MailInterceptors;

            Assert.NotNull(result);
            var mailInterceptors = result.ToArray();
            mailInterceptors.Length.ShouldEqual(1);
            mailInterceptors[0].DisplayName.ShouldEqual("UNCONFIGURED INTERCEPTORS");
            mailInterceptors[0].Address.ShouldEqual("intercept@localhost.tld");
        }

        [Fact]
        public void MailExceptionTo_ReturnsMailAddresses_BasedOnConfig()
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var appSettings = new NameValueCollection
            {
                {
                    "MailExceptionTo",
                    "Test Mail Exception To #1 Display Name <mail.exception.to.1@example.tld>;" +
                    "Test Mail Exception To #2 Display Name <mail.exception.to.2@example.tld>"
                },
            };
            mockConfigReader.SetupGet(x => x.AppSettings).Returns(appSettings);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);

            IEnumerable<MailAddress> result = appConfiguration.MailExceptionTo;

            Assert.NotNull(result);
            var mailExceptionTo = result.ToArray();
            mailExceptionTo.Length.ShouldEqual(2);
            mailExceptionTo[0].DisplayName.ShouldEqual("Test Mail Exception To #1 Display Name");
            mailExceptionTo[0].Address.ShouldEqual("mail.exception.to.1@example.tld");
            mailExceptionTo[1].DisplayName.ShouldEqual("Test Mail Exception To #2 Display Name");
            mailExceptionTo[1].Address.ShouldEqual("mail.exception.to.2@example.tld");
        }

        [Fact]
        public void MailExceptionTo_ReturnsPhonyMailAddresses_WhenNoConfigExists()
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var appSettings = new NameValueCollection();
            mockConfigReader.SetupGet(x => x.AppSettings).Returns(appSettings);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);

            IEnumerable<MailAddress> result = appConfiguration.MailExceptionTo;

            Assert.NotNull(result);
            var mailExceptionTo = result.ToArray();
            mailExceptionTo.Length.ShouldEqual(1);
            mailExceptionTo[0].DisplayName.ShouldEqual("UNCONFIGURED EXCEPTION");
            mailExceptionTo[0].Address.ShouldEqual("exceptions@localhost.tld");
        }

        [Fact]
        public void MailPickupDirectory_ReturnsValue_FromConfig()
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var appSettings = new NameValueCollection
            {
                {
                    "MailPickupDirectory",
                    @"special\custom/path"
                },
            };
            mockConfigReader.SetupGet(x => x.AppSettings).Returns(appSettings);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);

            string result = appConfiguration.MailPickupDirectory;
            result.ShouldEqual(@"special\custom/path");
        }

        [Fact]
        public void MailPickupDirectory_ReturnsDefaultValue_WhenNoConfigExists()
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var appSettings = new NameValueCollection();
            mockConfigReader.SetupGet(x => x.AppSettings).Returns(appSettings);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);

            string result = appConfiguration.MailPickupDirectory;
            result.ShouldEqual(@"App_Data\mail\pickup");
        }

        [Theory]
        [InlineData(SmtpDeliveryMethod.Network)]
        [InlineData(SmtpDeliveryMethod.PickupDirectoryFromIis)]
        [InlineData(SmtpDeliveryMethod.SpecifiedPickupDirectory)]
        public void MailDeliveryMethod_ReturnsValue_FromConfig(SmtpDeliveryMethod deliveryMethod)
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            var smtpSection = new SmtpSection
            {
                DeliveryMethod = deliveryMethod,
            };
            mockConfigReader.Setup(x => x.GetSection("system.net/mailSettings/smtp")).Returns(smtpSection);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);

            SmtpDeliveryMethod result = appConfiguration.MailDeliveryMethod;
            result.ShouldEqual(deliveryMethod);
        }

        [Fact]
        public void MailDeliveryMethod_ReturnsSpecifiedPickupDirectory_WhenNoConfigExists()
        {
            var mockConfigReader = new Mock<IReadConfiguration>(MockBehavior.Strict);
            mockConfigReader.Setup(x => x.GetSection("system.net/mailSettings/smtp"))
                .Returns(null as ConfigurationSection);
            var appConfiguration = new AppConfiguration(mockConfigReader.Object);

            SmtpDeliveryMethod result = appConfiguration.MailDeliveryMethod;
            result.ShouldEqual(SmtpDeliveryMethod.SpecifiedPickupDirectory);
        }
    }
}
