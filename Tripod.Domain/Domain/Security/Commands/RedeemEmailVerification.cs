using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class RedeemEmailVerification : BaseEntityCommand, IDefineSecuredCommand
    {
        [UsedImplicitly]
        public RedeemEmailVerification() { }

        internal RedeemEmailVerification(User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            User = user;
        }

        public IPrincipal Principal { get; set; }
        public string Token { get; set; }
        internal User User { get; private set; }
    }

    [UsedImplicitly]
    public class ValidateRedeemEmailVerificationCommand : AbstractValidator<RedeemEmailVerification>
    {
        public ValidateRedeemEmailVerificationCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .MustFindUserByPrincipal(queries)
                    .When(x => x.User == null)
                    .WithName(User.Constraints.Label);

            RuleFor(x => x.Token)
                .MustBeRedeemableVerifyEmailToken(queries)
                .MustBePurposedVerifyEmailToken(queries,
                    x => EmailVerificationPurpose.CreateRemoteUser,
                    x => EmailVerificationPurpose.CreateLocalUser,
                    x => EmailVerificationPurpose.AddEmail
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
            var user = command.User
                ?? await _entities.Get<User>()
                    .EagerLoad(x => x.EmailAddresses)
                    .ByIdAsync(command.Principal.Identity.GetAppUserId());
            email.Owner = user;
            email.IsVerified = true;

            // is this the user's primary email address?
            email.IsPrimary = !user.EmailAddresses.Any(x => x.IsPrimary);

            // expire unused verifications
            var unusedVerifications = await _entities.Get<EmailVerification>()
                .ByOwnerId(email.Id)
                .ToArrayAsync()
            ;
            foreach (var unusedVerification in unusedVerifications.Except(new[] { verification }))
                unusedVerification.RedeemedOnUtc = unusedVerification.ExpiresOnUtc;

            if (command.Commit) await _entities.SaveChangesAsync();
        }
    }
}
