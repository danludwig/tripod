using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class EmailConfirmationBy : BaseEntityQuery<EmailConfirmation>, IDefineQuery<Task<EmailConfirmation>>
    {
        public EmailConfirmationBy(string ticket)
        {
            Ticket = ticket;
        }

        public string Ticket { get; private set; }
    }

    [UsedImplicitly]
    public class HandleEmailConfirmationByQuery : IHandleQuery<EmailConfirmationBy, Task<EmailConfirmation>>
    {
        private readonly IReadEntities _entities;

        public HandleEmailConfirmationByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public async Task<EmailConfirmation> Handle(EmailConfirmationBy query)
        {
            var entity = await _entities.Query<EmailConfirmation>()
                .EagerLoad(query.EagerLoad)
                .ByTicketAsync(query.Ticket)
                .ConfigureAwait(false);
            return entity;
        }
    }
}
