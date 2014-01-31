using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class EmailVerificationBy : BaseEntityQuery<EmailVerification>, IDefineQuery<Task<EmailVerification>>
    {
        public EmailVerificationBy(string ticket)
        {
            Ticket = ticket;
        }

        public string Ticket { get; private set; }
    }

    [UsedImplicitly]
    public class HandleEmailVerificationByQuery : IHandleQuery<EmailVerificationBy, Task<EmailVerification>>
    {
        private readonly IReadEntities _entities;

        public HandleEmailVerificationByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public async Task<EmailVerification> Handle(EmailVerificationBy query)
        {
            var entity = await _entities.Query<EmailVerification>()
                .EagerLoad(query.EagerLoad)
                .ByTicketAsync(query.Ticket)
                .ConfigureAwait(false);
            return entity;
        }
    }
}
