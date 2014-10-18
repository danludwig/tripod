using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;

namespace Tripod
{
    [UsedImplicitly]
    public class AppConfiguration
    {
        private readonly IReadConfiguration _configuration;

        public AppConfiguration(IReadConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string XsrfKey
        {
            get { return _configuration.AppSettings[AppSettingKey.XsrfKey.ToString()]; }
        }

        public string DataProtectionAppName
        {
            get { return _configuration.AppSettings[AppSettingKey.DataProtectionAppName.ToString()] ?? "AppName"; }
        }

        public MailAddress MailFromDefault
        {
            get { return new MailAddress(_configuration.AppSettings[AppSettingKey.MailFromDefault.ToString()] ?? "UNCONFIGURED NOREPLY <no-reply@localhost.tld>"); }
        }

        public IEnumerable<MailAddress> MailInterceptors
        {
            get
            {
                return ExtractMailAddresses(_configuration.AppSettings[AppSettingKey.MailInterceptors.ToString()] ?? "UNCONFIGURED INTERCEPTORS <intercept@localhost.tld>");
            }
        }

        public IEnumerable<MailAddress> MailExceptionTo
        {
            get
            {
                return ExtractMailAddresses(_configuration.AppSettings[AppSettingKey.MailExceptionTo.ToString()] ?? "UNCONFIGURED EXCEPTION <exceptions@localhost.tld>");
            }
        }

        public string MailPickupDirectory
        {
            get { return _configuration.AppSettings[AppSettingKey.MailPickupDirectory.ToString()] ?? @"App_Data\mail\pickup"; }
        }

        public SmtpDeliveryMethod MailDeliveryMethod
        {
            get
            {
                var smtp = _configuration.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                return smtp == null ? SmtpDeliveryMethod.SpecifiedPickupDirectory : smtp.DeliveryMethod;
            }
        }

        private IEnumerable<MailAddress> ExtractMailAddresses(string collapsed)
        {
            return collapsed.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new MailAddress(x)).ToArray();
        }
    }
}
