using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class RedeemEmailVerification :  IDefineCommand
    {
        internal RedeemEmailVerification(string token, User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            Token = token;
            User = user;
        }

        internal string Token { get; private set; }
        internal User User { get; private set; }
    }

    [UsedImplicitly]
    public class ValidateRedeemEmailVerificationCommand : AbstractValidator<RedeemEmailVerification>
    {
        public ValidateRedeemEmailVerificationCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Token)
                .MustBeRedeemableVerifyEmailToken(queries)
                .MustBePurposedVerifyEmailToken(queries,
                    x => EmailVerificationPurpose.CreateRemoteUser,
                    x => EmailVerificationPurpose.CreateLocalUser
                )
                    .WithName(EmailVerification.Constraints.Label);
        }
    }

    [UsedImplicitly]
    public class HandleRedeemEmailVerificationCommand : IHandleCommand<RedeemEmailVerification>
    {
        private readonly IProcessQueries _queries;
        private readonly IWriteEntities _entities;

        public HandleRedeemEmailVerificationCommand(IProcessQueries queries, IWriteEntities entities)
        {
            _entities = entities;
            _queries = queries;
        }

        public async Task Handle(RedeemEmailVerification command)
        {
            // verify & associate email address
            var userToken = await _queries.Execute(new EmailVerificationUserToken(command.Token));
            var verification = await _entities.Get<EmailVerification>()
                .EagerLoad(x => x.Owner)
                .ByTicketAsync(userToken.Value, false);
            verification.RedeemedOnUtc = DateTime.UtcNow;

            var email = verification.Owner;
            email.Owner = command.User;
            email.IsVerified = true;

            // is this the user's primary email address?
            email.IsPrimary = !command.User.EmailAddresses.Any(x => x.IsPrimary);

            // expire unused verifications
            var unusedVerifications = await _entities.Get<EmailVerification>()
                .ByOwnerValue(email.Value)
                .ToArrayAsync()
            ;
            foreach (var unusedVerification in unusedVerifications.Except(new[] { verification }))
                unusedVerification.RedeemedOnUtc = unusedVerification.ExpiresOnUtc;
        }
    }
}
