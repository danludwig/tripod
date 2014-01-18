#if !DEBUG
using System;
#endif
using System.Net.Mail;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace Tripod.Ioc.Net
{
    public static class CompositionRoot
    {
        public static void RegisterMailDelivery(this Container container)
        {
            var mailDeliveryMethod = AppSettings.MailDeliveryMethod;
#if DEBUG
            if (mailDeliveryMethod != SmtpDeliveryMethod.Network)
            {
                // register the pickup directory implementation
                container.Register<IDeliverMailMessage, PickupDirectoryMailMessageDelivery>();
            }
            else
            {
                // register the smtp delivery implementation
                container.Register<IDeliverMailMessage, SmtpMailMessageDelivery>();
            }
#else
            if (mailDeliveryMethod != SmtpDeliveryMethod.Network)
                throw new InvalidOperationException(string.Format(
                    "Configuration setting system.net/mailSettings/smtp deliveryMethod '{0}' is not valid for release configuration.", mailDeliveryMethod));

            // register the smtp delivery implementation
            container.Register<IDeliverMailMessage, SmtpMailMessageDelivery>();
#endif

            // retry sending mail
            container.RegisterDecorator(
                typeof(IDeliverMailMessage),
                typeof(RetryDeliverMailDecorator)
            );

#if DEBUG
            if (mailDeliveryMethod == SmtpDeliveryMethod.Network)
            {
                container.RegisterDecorator(
                    typeof(IDeliverMailMessage),
                    typeof(InterceptMailDeliveryDecorator)
                );
            }
#endif
        }
    }
}
