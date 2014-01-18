using System.Configuration;
using System.Net.Mail;

namespace Tripod
{
    public static class AppSettings
    {
        public static MailAddress DefaultMailFrom
        {
            get
            {
                var appSetting = ConfigurationManager.AppSettings[AppSettingKey.DefaultMailFrom.ToString()] ??
                    "UNCONFIGURED NOREPLY <no-reply@localhost.tld>";

                var mailAddress = new MailAddress(appSetting);

                return mailAddress;
            }
        }
    }
}
