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
        public string Ticket { get; set; }
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

            // do not allow this command when purpose is to reset a forgotten password
            // because in that case, there is already an email associated with the user
            var validPurposes = new Func<RedeemEmailVerification, EmailVerificationPurpose>[]
            {
                x => EmailVerificationPurpose.CreateRemoteUser,
                x => EmailVerificationPurpose.CreateLocalUser,
                x => EmailVerificationPurpose.AddEmail,
            };
            RuleFor(x => x.Ticket)
                .MustBeRedeemableVerifyEmailTicket(queries)
                .MustBePurposedVerifyEmailTicket(queries, validPurposes)
                .MustHaveValidVerifyEmailToken(queries, x => x.Token)
                .WithName(EmailVerification.Constraints.Label)
            ;
        }
    }

    [UsedImplicitly]
    public class HandleRedeemEmailVerificationCommand : IHandleCommand<RedeemEmailVerification>
    {
        private readonly IWriteEntities _entities;

        public HandleRedeemEmailVerificationCommand(IWriteEntities entities)
        {
            _entities = entities;
        }

        public async Task Handle(RedeemEmailVerification command)
        {
            // verify & associate email address
            var verification = await _entities.Get<EmailVerification>()
                .EagerLoad(x => x.EmailAddress)
                .ByTicketAsync(command.Ticket, false);
            verification.RedeemedOnUtc = DateTime.UtcNow;

            var email = verification.EmailAddress;
            var user = command.User
                ?? await _entities.Get<User>()
                    .EagerLoad(x => x.EmailAddresses)
                    .ByIdAsync(command.Principal.Identity.GetAppUserId());
            email.User = user;
            email.IsVerified = true;

            // is this the user's primary email address?
            email.IsPrimary = !user.EmailAddresses.Any(x => x.IsPrimary);

            // expire unused verifications
            var unusedVerifications = await _entities.Get<EmailVerification>()
                .ByEmailAddressId(email.Id)
                .ToArrayAsync()
            ;
            foreach (var unusedVerification in unusedVerifications.Except(new[] { verification }))
                unusedVerification.RedeemedOnUtc = unusedVerification.ExpiresOnUtc;

            if (command.Commit) await _entities.SaveChangesAsync();
        }
    }
}
