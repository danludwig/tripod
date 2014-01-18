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
    }

    public class ValidateSendConfirmationEmailCommand : AbstractValidator<SendConfirmationEmail>
    {
        public ValidateSendConfirmationEmailCommand(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .EmailAddress()
                .MaxLength(EmailAddress.Constraints.ValueMaxLength)
                    .WithName(EmailAddress.Constraints.Label);

            RuleFor(x => x.IsExpectingEmail)
                .Equal(true)
                    .WithMessage(Resources.Validation_SendConfirmationEmail_IsExpectingEmail)
                        .WithName(EmailAddress.Constraints.Label.ToLower());

            // todo: email must not be confirmed if it exists
            // todo: what if the email address belongs to a user?
        }
    }

    public class HandleSendConfirmationEmailCommand : IHandleCommand<SendConfirmationEmail>
    {
        private readonly UserManager<User, int> _userManager;
        private readonly IProcessQueries _queries;
        private readonly IWriteEntities _entities;
        //private readonly IDeliverMailMessage _mail;

        public HandleSendConfirmationEmailCommand(UserManager<User, int> userManager, IProcessQueries queries, IWriteEntities entities)
        {
            _userManager = userManager;
            _queries = queries;
            _entities = entities;
            //_mail = mail;
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
            var token = _userManager.UserConfirmationTokens.Generate(new UserToken
            {
                UserId = command.EmailAddress,
                Value = secret,
                CreationDate = DateTime.UtcNow,
            });
            var confirmation = new EmailConfirmation
            {
                Owner = emailAddress,
                ExpiresOnUtc = DateTime.UtcNow.AddMinutes(30),
                Purpose = EmailConfirmationPurpose.CreatePassword,
                Secret = secret,
                Ticket = Guid.NewGuid().ToString(),
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
                // don't forget to encode + symbols in the ticket to %2b for querystring, otherwise they are space characters
                { "{ConfirmationUrl}", string.Format(command.ConfirmUrlFormat, confirmation.Token.Replace("+", "%2b")) },
                { "{SendFromUrl}", command.SendFromUrl }
            };

            // create the message
            var message = new EmailMessage
            {
                Owner = emailAddress,
                From = AppSettings.DefaultMailFrom.ToString(),
                Subject = formatters.Format(subjectFormat),
                Body = formatters.Format(bodyFormat),
                IsBodyHtml = false,
                SendOnUtc = DateTime.UtcNow,
            };
            _entities.Create(message);

            await _entities.SaveChangesAsync();
        }
    }
}
