using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using Elmah;

namespace Tripod.Ioc.Audit
{
    [UsedImplicitly]
    public class ElmahExceptionAuditor : IAuditException
    {
        private readonly IDeliverMailMessage _mailMessageSender;

        public ElmahExceptionAuditor(IDeliverMailMessage mailMessageSender)
        {
            _mailMessageSender = mailMessageSender;
        }

        public void Audit(Exception exception)
        {
            var error = new Error(exception);

            // first try to post it to the Elmah log
            try
            {
                var log = ErrorLog.GetDefault(HttpContext.Current);
                log.Log(error);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Failed to add exception to Elmah error logger: {0}", ex.Message), "Error");
            }

            // second try to send it via Elmah mail
            try
            {
                _mailMessageSender.Deliver(CreateMailMessage(exception));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Failed to send Elmah exception email: {0}", ex.Message), "Error");
            }
        }


        private MailMessage CreateMailMessage(Exception exception)
        {
            var error = new Error(exception);
            var subject = error.Message.Replace("\r", string.Empty).Replace("\n", string.Empty);
            var isHtml = true;
            var body = "An error occurred.";
            try
            {
                var writer = new StringWriter();
                var formatter = new ErrorMailHtmlFormatter();
                formatter.Format(writer, error);
                body = writer.ToString();
            }
            catch (Exception)
            {
                // some exceptions may creep up before we have access to the html formatter
                isHtml = false;
                var builder = new StringBuilder(body);
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine(exception.Message);
                builder.AppendLine();
                builder.AppendLine(exception.StackTrace);
                body = builder.ToString();
            }
            var tos = AppConfiguration.MailExceptionTo.ToArray();
            var mailMessage = new MailMessage(AppConfiguration.MailFromDefault, tos.First())
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml,
            };
            foreach (var to in tos.Skip(1)) mailMessage.To.Add(to);
            return mailMessage;
        }
    }
}
