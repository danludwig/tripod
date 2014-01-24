using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Authenticate user's local membership.
    /// </summary>
    public class SendConfirmationEmail : IDefineCommand
    {
        public string EmailAddress { get; set; }
        public bool IsExpectingEmail { get; set; }
        public string ConfirmUrlFormat { get; set; }
        public string SendFromUrl { get; set; }
        public string CreatedTicket { get; internal set; }
    }

    public class ValidateSendConfirmationEmailCommand : AbstractValidator<SendConfirmationEmail>
    {
        public ValidateSendConfirmationEmailCommand(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .EmailAddress()
                .MaxLength(EmailAddress.Constraints.ValueMaxLength)
                .MustNotBeConfirmedEmailAddress(queries)
                    .WithName(EmailAddress.Constraints.Label);

            RuleFor(x => x.IsExpectingEmail)
                .Equal(true)
                    .WithMessage(Resources.Validation_SendConfirmationEmail_IsExpectingEmail)
                        .WithName(EmailAddress.Constraints.Label.ToLower());
        }
    }

    public class HandleSendConfirmationEmailCommand : IHandleCommand<SendConfirmationEmail>
    {
        private readonly UserManager<User, int> _userManager;
        private readonly IProcessQueries _queries;
        private readonly IWriteEntities _entities;
        private readonly IDeliverEmailMessage _mail;

        public HandleSendConfirmationEmailCommand(UserManager<User, int> userManager, IProcessQueries queries, IWriteEntities entities, IDeliverEmailMessage mail)
        {
            _userManager = userManager;
            _queries = queries;
            _entities = entities;
            _mail = mail;
        }

        public async Task Handle(SendConfirmationEmail command)
        {
            // find or create the email address
            var emailAddress = await _entities.Get<EmailAddress>().ByValueAsync(command.EmailAddress)
                ?? new EmailAddress
                {
                    Value = command.EmailAddress,
                };

            // create the confirmation
            var secret = _queries.Execute(new RandomSecret(10, 12));
            var ticket = _queries.Execute(new RandomSecret(20, 25));

            // make sure ticket is unique, and does not collide with sibling routes
            while (_entities.Query<EmailConfirmation>().ByTicket(ticket) != null
                || "password".Equals(ticket, StringComparison.OrdinalIgnoreCase))
                ticket = _queries.Execute(new RandomSecret(20, 25));

            var token = _userManager.UserConfirmationTokens.Generate(new UserToken
            {
                UserId = command.EmailAddress,
                Value = ticket,
                CreationDate = DateTime.UtcNow,
            });
            var confirmation = new EmailConfirmation
            {
                Owner = emailAddress,
                ExpiresOnUtc = DateTime.UtcNow.AddMinutes(30),
                Purpose = EmailConfirmationPurpose.CreateLocalUser,
                Secret = secret,
                Ticket = ticket,
                Token = token,
            };
            _entities.Create(confirmation);

            // load the templates
            var resourceFormat = string.Format("{0}.{1}.txt", confirmation.Purpose, "{0}");
            var assembly = Assembly.GetExecutingAssembly();
            var subjectFormat = assembly.GetManifestResourceText(assembly.GetManifestResourceName(string.Format(resourceFormat, "Subject")));
            var bodyFormat = assembly.GetManifestResourceText(assembly.GetManifestResourceName(string.Format(resourceFormat, "Body")));

            // format the message body
            var formatters = new Dictionary<string, string>
            {
                { "{EmailAddress}", emailAddress.Value },
                { "{Secret}", confirmation.Secret },
                // don't forget to encode the token, it contains illegal querystring characters
                { "{ConfirmationUrl}", string.Format(command.ConfirmUrlFormat, Uri.EscapeDataString(confirmation.Token)) },
                { "{SendFromUrl}", command.SendFromUrl }
            };

            // create the message
            var message = new EmailMessage
            {
                Owner = emailAddress,
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
