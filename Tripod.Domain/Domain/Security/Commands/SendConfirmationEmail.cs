using System;
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
        private readonly IWriteEntities _entities;
        private readonly IProcessQueries _queries;

        public HandleSendConfirmationEmailCommand(UserManager<User, int> userManager, IWriteEntities entities, IProcessQueries queries)
        {
            _userManager = userManager;
            _entities = entities;
            _queries = queries;
        }

        public async Task Handle(SendConfirmationEmail command)
        {
            // first, find or create the email address
            var emailAddress = await _entities.Get<EmailAddress>().ByValueAsync(command.EmailAddress)
                ?? new EmailAddress
                {
                    Value = command.EmailAddress,
                };

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
                Purpose = EmailConfirmationPurpose.SignUp,
                Secret = secret,
                Ticket = Guid.NewGuid().ToString(),
                Token = token,
            };

            _entities.Create(confirmation);
            await _entities.SaveChangesAsync();
        }
    }
}
