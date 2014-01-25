using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class CreateEmailConfirmation : BaseCreateEntityCommand<EmailConfirmation>, IDefineCommand
    {
        public string EmailAddress { get; [UsedImplicitly] set; }
        public EmailConfirmationPurpose Purpose { get; [UsedImplicitly] set; }
    }

    public class ValidateCreateEmailConfirmationCommand : AbstractValidator<CreateEmailConfirmation>
    {
        public ValidateCreateEmailConfirmationCommand(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddress)
                .MustBeConfirmableEmailAddress(queries)
                    .WithName(EmailAddress.Constraints.Label);

            RuleFor(x => x.Purpose)
                .MustBeValidConfirmEmailPurpose()
                    .WithName(EmailConfirmation.Constraints.Label);
        }
    }

    [UsedImplicitly]
    public class HandleCreateEmailConfirmationCommand : IHandleCommand<CreateEmailConfirmation>
    {
        private readonly UserManager<User, int> _userManager;
        private readonly IProcessQueries _queries;
        private readonly IWriteEntities _entities;

        public HandleCreateEmailConfirmationCommand(UserManager<User, int> userManager, IProcessQueries queries, IWriteEntities entities)
        {
            _userManager = userManager;
            _queries = queries;
            _entities = entities;
        }

        public async Task Handle(CreateEmailConfirmation command)
        {
            // find or create the email address
            var emailAddress = await _entities.Get<EmailAddress>().ByValueAsync(command.EmailAddress)
                ?? new EmailAddress
                {
                    Value = command.EmailAddress,
                };

            // create random secret and ticket
            // note that changing the length of the secret requires updating the
            // email messages sent to the address, since those mention secret length
            var secret = _queries.Execute(new RandomSecret(10, 12));
            var ticket = _queries.Execute(new RandomSecret(20, 25));

            // make sure ticket is unique
            while (_entities.Query<EmailConfirmation>().ByTicket(ticket) != null)
                ticket = _queries.Execute(new RandomSecret(20, 25));

            // serialize a new user token to a string
            var token = _userManager.UserConfirmationTokens.Generate(new UserToken
            {
                UserId = command.EmailAddress,
                Value = ticket,
                CreationDate = DateTime.UtcNow,
            });

            // create the confirmation
            var confirmation = new EmailConfirmation
            {
                Owner = emailAddress,
                Purpose = command.Purpose,
                Secret = secret,
                Ticket = ticket,
                Token = token,

                // change this, and you have to change the content of the email messages to reflect new expiration
                ExpiresOnUtc = DateTime.UtcNow.AddMinutes(30),
            };
            _entities.Create(confirmation);

            if (command.Commit) await _entities.SaveChangesAsync();

            command.CreatedEntity = confirmation;
        }
    }
}
