using System.Linq;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class EmailAddressViewsBy : BaseEnumerableQuery<EmailAddressView>, IDefineQuery<Task<IQueryable<EmailAddressView>>>
    {
        [UsedImplicitly]
        public EmailAddressViewsBy(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; private set; }
        public bool? IsConfirmed { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class HandleEmailAddressViewsByQuery : IHandleQuery<EmailAddressViewsBy, Task<IQueryable<EmailAddressView>>>
    {
        private readonly IReadEntities _entities;

        public HandleEmailAddressViewsByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<IQueryable<EmailAddressView>> Handle(EmailAddressViewsBy query)
        {
            var queryable = _entities.Query<EmailAddress>()
                .Where(QueryEmailAddresses.ByUserId(query.UserId))
            ;

            if (query.IsConfirmed.HasValue)
                queryable = queryable.Where(x => x.IsConfirmed == query.IsConfirmed.Value);

            var projection = queryable
                .Select(x => new EmailAddressView
                {
                    EmailAddressId = x.Id,
                    UserId = x.OwnerId,
                    Value = x.Value,
                    HashedValue = x.HashedValue,
                    IsConfirmed = x.IsConfirmed,
                    IsDefault = x.IsDefault,
                })
                .OrderBy(query.OrderBy);

            return Task.FromResult(projection);
        }
    }
}
