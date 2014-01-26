using System.Security.Claims;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class EmailAddressBy : BaseEntityQuery<EmailAddress>, IDefineQuery<Task<EmailAddress>>
    {
        [UsedImplicitly]
        public EmailAddressBy(int id)
        {
            Id = id;
        }

        public EmailAddressBy(string value)
        {
            Value = value;
        }

        public EmailAddressBy(Claim claim)
        {
            Claim = claim;
        }

        public int? Id { get; private set; }
        public string Value { get; private set; }
        public Claim Claim { get; private set; }
        public bool? IsConfirmed { get; set; }
    }

    [UsedImplicitly]
    public class HandleEmailAddressByQuery : IHandleQuery<EmailAddressBy, Task<EmailAddress>>
    {
        private readonly IReadEntities _entities;

        public HandleEmailAddressByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public async Task<EmailAddress> Handle(EmailAddressBy query)
        {
            var queryable = _entities.Query<EmailAddress>()
                .EagerLoad(query.EagerLoad);

            Task<EmailAddress> entityTask;
            if (query.Id.HasValue)
            {
                entityTask = queryable.ByIdAsync(query.Id.Value);
            }
            else if (query.Claim != null && query.Claim.Type == ClaimTypes.Email)
            {
                entityTask = queryable.ByValueAsync(query.Claim.Value);
            }
            else
            {
                entityTask = queryable.ByValueAsync(query.Value);
            }

            var entity = await entityTask.ConfigureAwait(false);

            if (entity != null && query.IsConfirmed.HasValue && query.IsConfirmed.Value != entity.IsConfirmed)
                entity = null;

            return entity;
        }
    }
}
