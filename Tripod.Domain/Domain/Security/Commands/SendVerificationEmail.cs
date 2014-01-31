using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class SendVerificationEmail : IDefineCommand
    {
        public string EmailAddress { get; set; }
        public bool IsExpectingEmail { get; set; }
        public EmailVerificationPurpose Purpose { get; set; }
        public string VerifyUrlFormat { get; [UsedImplicitly] set; }
        public string SendFromUrl { get; [UsedImplicitly] set; }
        public string CreatedTicket { get; internal set; }
    }

    public class ValidateSendVerificationEmailCommand : AbstractValidator<SendVerificationEmail>
    {
        public ValidateSendVerificationEmailCommand(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddress)
                .MustBeVerifiableEmailAddress(queries)
                    .WithName(EmailAddress.Constraints.Label);

            RuleFor(x => x.IsExpectingEmail)
                .Equal(true)
                    .WithMessage(Resources.Validation_SendVerificationEmail_IsExpectingEmail)
                        .WithName(EmailAddress.Constraints.Label.ToLower());

            RuleFor(x => x.Purpose)
                .MustBeValidVerifyEmailPurpose()
                    .WithName(EmailVerification.Constraints.Label);

            RuleFor(x => x.VerifyUrlFormat)
                .NotEmpty()
                    .WithMessage(Resources.Validation_EmailVerification_MissingMessageFormatter)
                    .When(x => x.Purpose == EmailVerificationPurpose.CreateLocalUser);

            RuleFor(x => x.SendFromUrl)
                .NotEmpty()
                    .WithMessage(Resources.Validation_EmailVerification_MissingMessageFormatter);
        }
    }

    [UsedImplicitly]
    public class HandleSendVerificationEmailCommand : IHandleCommand<SendVerificationEmail>
    {
        private readonly IProcessCommands _commands;
        private readonly IWriteEntities _entities;
        private readonly IDeliverEmailMessage _mail;

        public HandleSendVerificationEmailCommand(IProcessCommands commands, IWriteEntities entities, IDeliverEmailMessage mail)
        {
            _commands = commands;
            _entities = entities;
            _mail = mail;
        }

        public async Task Handle(SendVerificationEmail command)
        {
            // create the email verification
            var createEmailVerification = new CreateEmailVerification
            {
                Commit = false,
                EmailAddress = command.EmailAddress,
                Purpose = command.Purpose,
            };
            await _commands.Execute(createEmailVerification);
            var verification = createEmailVerification.CreatedEntity;

            // load the templates
            var resourceFormat = string.Format("{0}.{1}.txt", verification.Purpose, "{0}");
            var assembly = Assembly.GetExecutingAssembly();
            var subjectFormat = assembly.GetManifestResourceText(assembly.GetManifestResourceName(string.Format(resourceFormat, "Subject")));
            var bodyFormat = assembly.GetManifestResourceText(assembly.GetManifestResourceName(string.Format(resourceFormat, "Body")));

            // format the message body
            var formatters = new Dictionary<string, string>
            {
                { "{EmailAddress}", verification.Owner.Value },
                { "{Secret}", verification.Secret },
                // don't forget to encode the token, it contains illegal querystring characters
                { "{VerificationUrl}", string.Format(command.VerifyUrlFormat ?? "", Uri.EscapeDataString(verification.Token)) },
                { "{SendFromUrl}", command.SendFromUrl }
            };

            // create the message
            var message = new EmailMessage
            {
                Owner = verification.Owner,
                From = AppConfiguration.MailFromDefault.ToString(),
                Subject = formatters.Format(subjectFormat),
                Body = formatters.Format(bodyFormat),
                IsBodyHtml = false,
                SendOnUtc = DateTime.UtcNow,
            };
            _entities.Create(message);

            // link the message to the verification
            verification.Message = message;

            await _entities.SaveChangesAsync();

            _mail.Deliver(message.Id);
            command.CreatedTicket = verification.Ticket;
        }
    }
}
