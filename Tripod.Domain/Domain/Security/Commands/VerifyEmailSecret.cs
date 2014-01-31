using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class VerifyEmailSecret : IDefineCommand
    {
        public string Secret
        {
            get { return _secret; }
            [UsedImplicitly] set { _secret = value != null ? value.Trim() : null; }
        }
        private string _secret;
        public string Ticket { get; [UsedImplicitly] set; }
        public EmailVerificationPurpose Purpose { get; [UsedImplicitly] set; }
        public string Token { get; internal set; }
    }

    [UsedImplicitly]
    public class ValidateVerifyEmailSecretCommand : AbstractValidator<VerifyEmailSecret>
    {
        public ValidateVerifyEmailSecretCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Ticket)
                .NotEmpty()
                .MustFindEmailVerificationByTicket(queries)
                .MustNotBeRedeemedVerifyEmailTicket(queries)
                .MustNotBeExpiredVerifyEmailTicket(queries)
                .MustBePurposedVerifyEmailTicket(x => x.Purpose, queries)
                    .WithName(EmailVerification.Constraints.Label);

            RuleFor(x => x.Secret)
                .NotEmpty()
                .MustBeVerifiedEmailSecret(x => x.Ticket, queries)
                    .WithName(EmailVerification.Constraints.SecretLabel);
        }
    }

    [UsedImplicitly]
    public class HandleVerifyEmailSecretCommand : IHandleCommand<VerifyEmailSecret>
    {
        private readonly IReadEntities _entities;

        public HandleVerifyEmailSecretCommand(IWriteEntities entities)
        {
            _entities = entities;
        }

        public async Task Handle(VerifyEmailSecret command)
        {
            var entity = await _entities.Query<EmailVerification>().ByTicketAsync(command.Ticket);
            command.Token = entity.Token;
        }
    }
}
