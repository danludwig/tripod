using System;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace Tripod.Services.Net
{
    [UsedImplicitly]
    public class PickupDirectoryMailMessageDelivery : SmtpMailMessageDelivery
    {
        private readonly AppConfiguration _appConfiguration;

        public PickupDirectoryMailMessageDelivery(AppConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }

        public override void Deliver(MailMessage message, SendCompletedEventHandler sendCompleted = null, object userState = null)
        {
            // deliver mail to pickup folder instead of over network
            var pickupDirectory = Path.Combine(_appConfiguration.MailPickupDirectory, message.To.First().Address);
            var directory = Directory.CreateDirectory(AppDomain.CurrentDomain.GetFullPath(pickupDirectory));
            SmtpClientInstance.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            SmtpClientInstance.PickupDirectoryLocation = directory.FullName;
            base.Deliver(message, sendCompleted, userState);
        }
    }
}
