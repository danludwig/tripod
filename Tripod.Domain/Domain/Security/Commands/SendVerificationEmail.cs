using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class SendVerificationEmail : IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
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
            RuleFor(x => x.Principal)
                .MustBeUnauthenticatedPrincipal()
                    .When(x => x.Purpose == EmailVerificationPurpose.CreateLocalUser
                            || x.Purpose == EmailVerificationPurpose.CreateRemoteUser
                            || x.Purpose == EmailVerificationPurpose.ForgotPassword,
                        ApplyConditionTo.CurrentValidator)
                .MustFindUserByPrincipal(queries)
                    .When(x => x.Purpose == EmailVerificationPurpose.AddEmail,
                        ApplyConditionTo.CurrentValidator)
            ;

            RuleFor(x => x.EmailAddress)
                .MustBeVerifiableEmailAddress(queries)
                    .When(x => x.Purpose != EmailVerificationPurpose.ForgotPassword, ApplyConditionTo.CurrentValidator)
                .MustFindUserByVerifiedEmail(queries)
                    .When(x => x.Purpose == EmailVerificationPurpose.ForgotPassword, ApplyConditionTo.CurrentValidator)
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
                // do not require a verification url when registering remote users,
                // but do require one when registering local users and adding new emails to user account
                .When(x => x.Purpose != EmailVerificationPurpose.CreateRemoteUser)
                .WithMessage(Resources.Validation_EmailVerification_MissingMessageFormatter)
            ;

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
        private readonly AppConfiguration _appConfiguration;

        public HandleSendVerificationEmailCommand(
              IProcessCommands commands
            , IWriteEntities entities
            , IDeliverEmailMessage mail
            , AppConfiguration appConfiguration)
        {
            _commands = commands;
            _entities = entities;
            _mail = mail;
            _appConfiguration = appConfiguration;
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

            // attach the email to a user when appropriate
            if (command.Purpose == EmailVerificationPurpose.AddEmail && command.Principal != null && command.Principal.Identity.IsAuthenticated)
                verification.EmailAddress.UserId = command.Principal.Identity.GetUserId<int>();

            // load the templates
            var resourceFormat = string.Format("{0}.{1}.txt", verification.Purpose, "{0}");
            var assembly = Assembly.GetExecutingAssembly();
            var subjectFormat = assembly.GetManifestResourceText(assembly.GetManifestResourceName(string.Format(resourceFormat, "Subject")));
            var bodyFormat = assembly.GetManifestResourceText(assembly.GetManifestResourceName(string.Format(resourceFormat, "Body")));

            // format the message body
            var formatters = new Dictionary<string, string>
            {
                { "{EmailAddress}", verification.EmailAddress.Value },
                { "{Secret}", verification.Secret },
                // don't forget to encode the token, it contains illegal querystring characters
                { "{VerificationUrl}", string.Format(command.VerifyUrlFormat ?? "",
                    Uri.EscapeDataString(verification.Token),
                    Uri.EscapeDataString(verification.Ticket)) },
                { "{SendFromUrl}", command.SendFromUrl }
            };

            // create the message
            var message = new EmailMessage
            {
                EmailAddress = verification.EmailAddress,
                From = _appConfiguration.MailFromDefault.ToString(),
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
