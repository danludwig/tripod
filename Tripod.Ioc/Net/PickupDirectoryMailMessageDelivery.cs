using System;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace Tripod.Ioc.Net
{
    public class PickupDirectoryMailMessageDelivery : SmtpMailMessageDelivery
    {
        public override void Deliver(MailMessage message, SendCompletedEventHandler sendCompleted = null, object userState = null)
        {
            // deliver mail to pickup folder instead of over network
            var pickupDirectory = Path.Combine(AppConfiguration.MailPickupDirectory, message.To.First().Address);
            var directory = Directory.CreateDirectory(AppDomain.CurrentDomain.GetFullPath(pickupDirectory));
            SmtpClientInstance.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            SmtpClientInstance.PickupDirectoryLocation = directory.FullName;
            base.Deliver(message, sendCompleted, userState);
        }
    }
}
