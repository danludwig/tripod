using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class RedeemEmailConfirmation :  IDefineCommand
    {
        internal RedeemEmailConfirmation(string token, User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            Token = token;
            User = user;
        }

        internal string Token { get; private set; }
        internal User User { get; private set; }
    }

    [UsedImplicitly]
    public class ValidateRedeemEmailConfirmationCommand : AbstractValidator<RedeemEmailConfirmation>
    {
        public ValidateRedeemEmailConfirmationCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .MustBeValidConfirmEmailToken(queries)
                .MustFindEmailConfirmationByToken(queries)
                .MustNotBeRedeemedConfirmEmailToken(queries)
                .MustNotBeExpiredConfirmEmailToken(queries)
                .MustBePurposedConfirmEmailToken(queries,
                    x => EmailConfirmationPurpose.CreateRemoteUser,
                    x => EmailConfirmationPurpose.CreateLocalUser
                )
                    .WithName(EmailConfirmation.Constraints.Label);
        }
    }

    [UsedImplicitly]
    public class HandleRedeemEmailConfirmationCommand : IHandleCommand<RedeemEmailConfirmation>
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;
        private readonly IWriteEntities _entities;

        public HandleRedeemEmailConfirmationCommand(IProcessQueries queries, IProcessCommands commands, IWriteEntities entities)
        {
            _commands = commands;
            _entities = entities;
            _queries = queries;
        }

        public async Task Handle(RedeemEmailConfirmation command)
        {
            // confirm & associate email address
            var userToken = await _queries.Execute(new EmailConfirmationUserToken(command.Token));
            var confirmation = await _entities.Get<EmailConfirmation>()
                .EagerLoad(x => x.Owner)
                .ByTicketAsync(userToken.Value, false);
            confirmation.RedeemedOnUtc = DateTime.UtcNow;

            var email = confirmation.Owner;
            email.Owner = command.User;
            email.IsConfirmed = true;

            // is this the user's default email address?
            email.IsDefault = !command.User.EmailAddresses.Any(x => x.IsDefault);

            // expire unused confirmations
            var unusedConfirmations = await _entities.Get<EmailConfirmation>()
                .ByOwnerValue(email.Value)
                .ToArrayAsync()
            ;
            foreach (var unusedConfirmation in unusedConfirmations.Except(new[] { confirmation }))
                unusedConfirmation.RedeemedOnUtc = unusedConfirmation.ExpiresOnUtc;
        }
    }
}
