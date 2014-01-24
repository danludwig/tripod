using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class EmailAddressBy : BaseEntityQuery<EmailAddress>, IDefineQuery<Task<EmailAddress>>
    {
        public EmailAddressBy(int id)
        {
            Id = id;
        }

        public EmailAddressBy(string value)
        {
            Value = value;
        }

        public int? Id { get; private set; }
        public string Value { get; private set; }
    }

    public class HandleEmailAddressByQuery : IHandleQuery<EmailAddressBy, Task<EmailAddress>>
    {
        private readonly IReadEntities _entities;

        public HandleEmailAddressByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<EmailAddress> Handle(EmailAddressBy query)
        {
            var queryable = _entities.Query<EmailAddress>()
                .EagerLoad(query.EagerLoad);

            if (query.Id.HasValue) return queryable.ByIdAsync(query.Id.Value);

            return queryable.ByValueAsync(query.Value);
        }
    }
}
