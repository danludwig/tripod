using System;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class ResetPassword : IDefineCommand
    {
        public string Token { get; [UsedImplicitly] set; }
        public string Ticket { get; [UsedImplicitly] set; }
        public string Password { get; [UsedImplicitly] set; }
        public string ConfirmPassword { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class ValidateResetPasswordCommand : AbstractValidator<ResetPassword>
    {
        public ValidateResetPasswordCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Ticket)
                .MustBeRedeemableVerifyEmailTicket(queries)
                .MustBePurposedVerifyEmailTicket(queries, x => EmailVerificationPurpose.ForgotPassword)
                .MustHaveValidVerifyEmailToken(queries, x => x.Token)
                .WithName(EmailVerification.Constraints.Label)
            ;

            RuleFor(x => x.Password)
                .MustBeValidPassword()
                .WithName(LocalMembership.Constraints.PasswordLabel)
            ;

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .MustEqualPassword(x => x.Password, LocalMembership.Constraints.PasswordLabel)
                    // only needs to equal password when password is not empty or whitespace
                    .When(x => !string.IsNullOrWhiteSpace(x.Password),
                        ApplyConditionTo.CurrentValidator)
                .WithName(LocalMembership.Constraints.PasswordConfirmationLabel)
            ;
        }
    }

    [UsedImplicitly]
    public class HandleResetPasswordCommand : IHandleCommand<ResetPassword>
    {
        private readonly IProcessQueries _queries;
        private readonly IWriteEntities _entities;

        public HandleResetPasswordCommand(IProcessQueries queries, IWriteEntities entities)
        {
            _entities = entities;
            _queries = queries;
        }

        public async Task Handle(ResetPassword command)
        {
            var verification = await _entities.Get<EmailVerification>()
                .EagerLoad(x => x.EmailAddress)
                .ByTicketAsync(command.Ticket, false);
            verification.RedeemedOnUtc = DateTime.UtcNow;
            var user = verification.EmailAddress.User;

            var localMembership = await _queries.Execute(new LocalMembershipByVerifiedEmail(verification.EmailAddress.Value));

            // if the user has a no local membership, create one
            if (localMembership == null)
            {
                localMembership = new LocalMembership { User = user, };
                user.LocalMembership = localMembership;
            }

            // update the password
            var passwordHash = await _queries.Execute(new HashedPassword(command.Password));
            user.LocalMembership.PasswordHash = passwordHash;

            user.SecurityStamp = Guid.NewGuid().ToString();
            await _entities.SaveChangesAsync();
        }
    }
}
