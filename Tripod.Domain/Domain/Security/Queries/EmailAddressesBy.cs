using System.Linq;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class EmailAddressesBy : BaseEntitiesQuery<EmailAddress>, IDefineQuery<Task<IQueryable<EmailAddress>>>
    {
        [UsedImplicitly]
        public EmailAddressesBy(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; private set; }
        public bool? IsConfirmed { get; set; }
    }

    [UsedImplicitly]
    public class HandleEmailAddressesByQuery : IHandleQuery<EmailAddressesBy, Task<IQueryable<EmailAddress>>>
    {
        private readonly IReadEntities _entities;

        public HandleEmailAddressesByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<IQueryable<EmailAddress>> Handle(EmailAddressesBy query)
        {
            var queryable = _entities.Query<EmailAddress>()
                .EagerLoad(query.EagerLoad)
                .ByUserId(query.UserId);

            if (query.IsConfirmed.HasValue)
                queryable = queryable.Where(x => x.IsConfirmed == query.IsConfirmed.Value);

            return Task.FromResult(queryable);
        }
    }
}
