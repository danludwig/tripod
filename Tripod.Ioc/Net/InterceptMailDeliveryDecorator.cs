using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Tripod.Ioc.Net
{
    public class InterceptMailDeliveryDecorator : IDeliverMailMessage
    {
        private readonly IDeliverMailMessage _decorated;

        public InterceptMailDeliveryDecorator(IDeliverMailMessage decorated)
        {
            _decorated = decorated;
        }

        public void Deliver(MailMessage message, SendCompletedEventHandler sendCompleted = null, object userState = null)
        {
            const string messageFormat =
@"
***********************************************
* This message was intercepted before it was
* sent over the network. The intended
* recipients were:
* {0}
***********************************************
";
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("TO:");
            AppendIntendedRecipients(message.To, messageBuilder);

            if (message.CC.Any())
            {
                messageBuilder.AppendLine("* CC:");
                AppendIntendedRecipients(message.CC, messageBuilder);
            }

            if (message.Bcc.Any())
            {
                messageBuilder.AppendLine("* BCC:");
                AppendIntendedRecipients(message.Bcc, messageBuilder);
            }

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            foreach (var interceptor in AppConfiguration.MailInterceptors)
                message.To.Add(interceptor);

            var formattedMessage = string.Format(messageFormat, messageBuilder.ToString().Trim());
            message.Body = string.Format("{0}{1}", formattedMessage, message.Body);

            _decorated.Deliver(message, sendCompleted, userState);
        }

        private static void AppendIntendedRecipients(IEnumerable<MailAddress> recipients, StringBuilder messageBuilder)
        {
            foreach (var recipient in recipients)
            {
                if (!string.IsNullOrWhiteSpace(recipient.DisplayName) && recipient.DisplayName != recipient.Address)
                    messageBuilder.AppendLine(string.Format("* {0} <{1}>", recipient.DisplayName, recipient.Address));
                else
                    messageBuilder.AppendLine(string.Format("* {0}", recipient.Address));
            }
        }
    }
}
