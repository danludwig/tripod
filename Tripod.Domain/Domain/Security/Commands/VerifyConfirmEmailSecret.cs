using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Authenticate user's local membership.
    /// </summary>
    public class VerifyConfirmEmailSecret : IDefineCommand
    {
        public string Secret
        {
            get { return _secret; }
            set { _secret = value != null ? value.Trim() : null; }
        }
        private string _secret;
        public string Ticket { get; set; }
        public EmailConfirmationPurpose Purpose { get; set; }
        public string Token { get; internal set; }
    }

    public class ValidateVerifyConfirmEmailSecretCommand : AbstractValidator<VerifyConfirmEmailSecret>
    {
        public ValidateVerifyConfirmEmailSecretCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Ticket)
                .NotEmpty()
                .MustFindEmailConfirmationByTicket(queries)
                .MustNotBeExpiredConfirmationTicket(queries)
                .MustNotBeRedeemedConfirmationTicket(queries)
                .MustBePurposedConfirmationTicket(x => x.Purpose, queries)
                    .WithName(EmailConfirmation.Constraints.Label);

            RuleFor(x => x.Secret)
                .NotEmpty()
                .MustBeVerifiedConfirmationSecret(x => x.Ticket, queries)
                    .WithName(EmailConfirmation.Constraints.SecretLabel);
        }
    }

    public class HandleVerifyConfirmEmailSecretCommand : IHandleCommand<VerifyConfirmEmailSecret>
    {
        private readonly IReadEntities _entities;

        public HandleVerifyConfirmEmailSecretCommand(IWriteEntities entities)
        {
            _entities = entities;
        }

        public async Task Handle(VerifyConfirmEmailSecret command)
        {
            var entity = await _entities.Query<EmailConfirmation>().ByTicketAsync(command.Ticket);
            command.Token = entity.Token;
        }
    }
}
