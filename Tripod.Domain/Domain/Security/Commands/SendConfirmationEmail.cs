using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class SendConfirmationEmail : IDefineCommand
    {
        public string EmailAddress { get; set; }
        public bool IsExpectingEmail { get; set; }
        public EmailConfirmationPurpose Purpose { get; set; }
        public string ConfirmUrlFormat { get; [UsedImplicitly] set; }
        public string SendFromUrl { get; [UsedImplicitly] set; }
        public string CreatedTicket { get; internal set; }
    }

    public class ValidateSendConfirmationEmailCommand : AbstractValidator<SendConfirmationEmail>
    {
        public ValidateSendConfirmationEmailCommand(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddress)
                .MustBeConfirmableEmailAddress(queries)
                    .WithName(EmailAddress.Constraints.Label);

            RuleFor(x => x.IsExpectingEmail)
                .Equal(true)
                    .WithMessage(Resources.Validation_SendConfirmationEmail_IsExpectingEmail)
                        .WithName(EmailAddress.Constraints.Label.ToLower());

            RuleFor(x => x.Purpose)
                .MustBeValidConfirmEmailPurpose()
                    .WithName(EmailConfirmation.Constraints.Label);

            RuleFor(x => x.ConfirmUrlFormat)
                .NotEmpty()
                    .WithMessage(Resources.Validation_EmailConfirmation_MissingMessageFormatter)
                    .When(x => x.Purpose == EmailConfirmationPurpose.CreateLocalUser);

            RuleFor(x => x.SendFromUrl)
                .NotEmpty()
                    .WithMessage(Resources.Validation_EmailConfirmation_MissingMessageFormatter);
        }
    }

    [UsedImplicitly]
    public class HandleSendConfirmationEmailCommand : IHandleCommand<SendConfirmationEmail>
    {
        private readonly IProcessCommands _commands;
        private readonly IProcessQueries _queries;
        private readonly IWriteEntities _entities;
        private readonly IDeliverEmailMessage _mail;

        public HandleSendConfirmationEmailCommand(IProcessCommands commands, IProcessQueries queries, IWriteEntities entities, IDeliverEmailMessage mail)
        {
            _commands = commands;
            _queries = queries;
            _entities = entities;
            _mail = mail;
        }

        public async Task Handle(SendConfirmationEmail command)
        {
            // create the email confirmation
            var createEmailConfirmation = new CreateEmailConfirmation
            {
                Commit = false,
                EmailAddress = command.EmailAddress,
                Purpose = command.Purpose,
            };
            await _commands.Execute(createEmailConfirmation);
            var confirmation = createEmailConfirmation.CreatedEntity;

            // load the templates
            var resourceFormat = string.Format("{0}.{1}.txt", confirmation.Purpose, "{0}");
            var assembly = Assembly.GetExecutingAssembly();
            var subjectFormat = assembly.GetManifestResourceText(assembly.GetManifestResourceName(string.Format(resourceFormat, "Subject")));
            var bodyFormat = assembly.GetManifestResourceText(assembly.GetManifestResourceName(string.Format(resourceFormat, "Body")));

            // format the message body
            var formatters = new Dictionary<string, string>
            {
                { "{EmailAddress}", confirmation.Owner.Value },
                { "{Secret}", confirmation.Secret },
                // don't forget to encode the token, it contains illegal querystring characters
                { "{ConfirmationUrl}", string.Format(command.ConfirmUrlFormat ?? "", Uri.EscapeDataString(confirmation.Token)) },
                { "{SendFromUrl}", command.SendFromUrl }
            };

            // create the message
            var message = new EmailMessage
            {
                Owner = confirmation.Owner,
                From = AppConfiguration.MailFromDefault.ToString(),
                Subject = formatters.Format(subjectFormat),
                Body = formatters.Format(bodyFormat),
                IsBodyHtml = false,
                SendOnUtc = DateTime.UtcNow,
            };
            _entities.Create(message);

            // link the message to the confirmation
            confirmation.Message = message;

            await _entities.SaveChangesAsync();

            _mail.Deliver(message.Id);
            command.CreatedTicket = confirmation.Ticket;
        }
    }
}
