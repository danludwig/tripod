using System;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class ResetPassword : IDefineCommand
    {
        public string Token { get; [UsedImplicitly] set; }
        public string Password { get; [UsedImplicitly] set; }
        public string ConfirmPassword { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class ValidateResetPasswordCommand : AbstractValidator<ResetPassword>
    {
        public ValidateResetPasswordCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Token)
                .MustBeRedeemableVerifyEmailToken(queries)
                .MustBePurposedVerifyEmailToken(queries,
                    x => EmailVerificationPurpose.ForgotPassword
                )
                    .WithName(EmailVerification.Constraints.Label);

            RuleFor(x => x.Password)
                .MustBeValidPassword()
                    .WithName(LocalMembership.Constraints.PasswordLabel)
            ;

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .MustEqualPassword(x => x.Password, LocalMembership.Constraints.PasswordLabel)
                    .WithName(LocalMembership.Constraints.PasswordConfirmationLabel)
                .When(x => !string.IsNullOrWhiteSpace(x.Password), ApplyConditionTo.CurrentValidator);
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
            var userToken = await _queries.Execute(new EmailVerificationUserToken(command.Token));
            var verification = await _entities.Get<EmailVerification>()
                .EagerLoad(x => x.EmailAddress)
                .ByTicketAsync(userToken.Value, false);
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
