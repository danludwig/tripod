using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Authenticate user's local membership.
    /// </summary>
    public class VerifyConfirmEmailSecret : IDefineCommand
    {
        public string Ticket { get; set; }
        public string Secret { get; set; }
        public EmailConfirmationPurpose Purpose { get; set; }
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
                    .WithName(EmailConfirmation.Constraints.TicketLabel);

            RuleFor(x => x.Secret)
                .NotEmpty()
                .MustBeVerifiedConfirmationSecret(x => x.Ticket, queries)
                    .WithName(EmailConfirmation.Constraints.SecretLabel);
        }
    }

    public class HandleVerifyConfirmEmailSecretCommand : IHandleCommand<VerifyConfirmEmailSecret>
    {
        //private readonly UserManager<User, int> _userManager;
        //private readonly IProcessQueries _queries;
        private readonly IReadEntities _entities;
        //private readonly IDeliverEmailMessage _mail;

        public HandleVerifyConfirmEmailSecretCommand(IWriteEntities entities)
        {
            //_userManager = userManager;
            //_queries = queries;
            _entities = entities;
            //_mail = mail;
        }

        public async Task Handle(VerifyConfirmEmailSecret command)
        {
        }
    }
}
