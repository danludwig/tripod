using System;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;

namespace Tripod
{
    public static class AppConfiguration
    {
        public static MailAddress MailFromDefault
        {
            get { return new MailAddress(ConfigurationManager.AppSettings[AppSettingKey.MailFromDefault.ToString()] ?? "UNCONFIGURED NOREPLY <no-reply@localhost.tld>"); }
        }

        public static MailAddress[] MailInterceptors
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppSettingKey.MailInterceptors.ToString()] ?? "UNCONFIGURED INTERCEPTORS <intercept@localhost.tld>";
                var intercepts = setting.Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x));
                return intercepts.Select(x => new MailAddress(x)).ToArray();
            }
        }

        public static string MailPickupDirectory
        {
            get { return ConfigurationManager.AppSettings[AppSettingKey.MailPickupDirectory.ToString()] ?? @"App_Data\mail\pickup"; }
        }

        public static SmtpDeliveryMethod MailDeliveryMethod
        {
            get
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                return smtp == null ? SmtpDeliveryMethod.SpecifiedPickupDirectory : smtp.DeliveryMethod;
            }
        }
    }
}
