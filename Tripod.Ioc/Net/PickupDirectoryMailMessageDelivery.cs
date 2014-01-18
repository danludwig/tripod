using System;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace Tripod.Ioc.Net
{
    public class PickupDirectoryMailMessageDelivery : SmtpMailMessageDelivery
    {
        public override void Deliver(MailMessage message)
        {
            // deliver mail to pickup folder instead of over network
            var pickupDirectory = Path.Combine(AppSettings.MailPickupDirectory, message.To.First().Address);
            var directory = Directory.CreateDirectory(AppDomain.CurrentDomain.GetFullPath(pickupDirectory));
            SmtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            SmtpClient.PickupDirectoryLocation = directory.FullName;
            base.Deliver(message);
        }
    }
}
