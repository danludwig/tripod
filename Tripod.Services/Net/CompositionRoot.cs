#if !DEBUG
using System;
#endif
using System.Net.Mail;
using SimpleInjector;
using SimpleInjector.Extensions;
using Tripod.Services.Configuration;

namespace Tripod.Services.Net
{
    public static class CompositionRoot
    {
        public static void RegisterMailDelivery(this Container container)
        {
            // mail delivery is separated into 2 distinct sets of services:
            // the first is actual MAIL delivery, which deals with objects
            // like MailMessage and SmtpClient in the system.net namespace.
            // the second set is EMAIL delivery, which deals with the
            // EmailMessage application entity, and tracks delivery status.
            // the app should consume IDeliverEmailMessage, whose implementations
            // will consume network delivery & status tracking internally

            #region Mail Delivery

            var appConfiguration = new AppConfiguration(new ConfigurationManagerReader());
            var mailDeliveryMethod = appConfiguration.MailDeliveryMethod;
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
            // when sending over the network in debug mode, intercept mail
            if (mailDeliveryMethod == SmtpDeliveryMethod.Network)
            {
                container.RegisterDecorator(
                    typeof(IDeliverMailMessage),
                    typeof(InterceptMailDeliveryDecorator)
                );
            }
#endif

            #endregion
            #region EmailMessage Delivery

            // send email in a new thread
            container.Register<IDeliverEmailMessage, ActiveEmailMessageDelivery>();
            container.RegisterDecorator(
                typeof(IDeliverEmailMessage),
                typeof(DeliverEmailAsynchronouslyDecorator)
            );

            // respond to email delivery result in a new thread
            container.Register<IDeliveredEmailMessage, OnEmailMessageDelivery>();
            container.RegisterDecorator(
                typeof(IDeliveredEmailMessage),
                typeof(DeliveredEmailAsynchronouslyDecorator)
            );

            #endregion
        }
    }
}
