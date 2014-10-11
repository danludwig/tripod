using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class RejectEmailVerification : BaseEntityCommand, IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
        public string Token { get; [UsedImplicitly] set; }
        public string Ticket { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class ValidateRejectEmailVerificationCommand : AbstractValidator<RejectEmailVerification>
    {
        public ValidateRejectEmailVerificationCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .MustFindUserByPrincipal(queries)
                    .WithName(User.Constraints.Label)
            ;

            RuleFor(x => x.Ticket)
                .MustBeRedeemableVerifyEmailTicket(queries)
                .MustBePurposedVerifyEmailTicket(queries,
                    x => EmailVerificationPurpose.AddEmail
                )
                    .WithName(EmailVerification.Constraints.Label)
            ;

            RuleFor(x => x.Token)
                .MustBeValidVerifyEmailToken(queries, x => x.Ticket)
                .WithName(EmailVerification.Constraints.Label)
            ;
        }
    }

    [UsedImplicitly]
    public class HandleRejectEmailVerificationCommand : IHandleCommand<RejectEmailVerification>
    {
        private readonly IProcessQueries _queries;
        private readonly IWriteEntities _entities;

        public HandleRejectEmailVerificationCommand(IProcessQueries queries, IWriteEntities entities)
        {
            _entities = entities;
            _queries = queries;
        }

        public async Task Handle(RejectEmailVerification command)
        {
            // reject this email verification
            var verification = await _entities.Get<EmailVerification>()
                .EagerLoad(x => x.EmailAddress)
                .ByTicketAsync(command.Ticket, false);
            verification.Token = "Rejected";

            var email = verification.EmailAddress;
            email.UserId = null;
            verification.Secret = null;
            var ticket = _queries.Execute(new RandomSecret(20, 25));

            // make sure ticket is unique
            while (_entities.Query<EmailVerification>().ByTicket(ticket) != null)
                ticket = _queries.Execute(new RandomSecret(20, 25));
            verification.Ticket = ticket;

            await _entities.SaveChangesAsync();
        }
    }
}
