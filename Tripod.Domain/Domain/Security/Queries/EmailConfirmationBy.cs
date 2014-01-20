using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class EmailConfirmationBy : BaseEntityQuery<EmailConfirmation>, IDefineQuery<Task<EmailConfirmation>>
    {
        public EmailConfirmationBy(string ticket, string token = null)
        {
            Ticket = ticket;
            Token = token;
        }

        public string Ticket { get; private set; }
        public string Token { get; private set; }
    }

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
                .ByTicketAsync(query.Ticket);
            if (entity == null) return null;
            if (!string.IsNullOrWhiteSpace(query.Token) && query.Token != entity.Token) return null;
            return entity;
        }
    }
}
