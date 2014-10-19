using System.Linq;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class EmailAddressViewsBy : BaseEnumerableQuery<EmailAddressView>, IDefineQuery<Task<IQueryable<EmailAddressView>>>
    {
        public EmailAddressViewsBy(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; private set; }
        public bool? IsVerified { get; set; }
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

            if (query.IsVerified.HasValue)
                queryable = queryable.Where(x => x.IsVerified == query.IsVerified.Value);

            var projection = queryable
                .Select(x => new EmailAddressView
                {
                    EmailAddressId = x.Id,
                    UserId = x.UserId,
                    Value = x.Value,
                    HashedValue = x.HashedValue,
                    IsVerified = x.IsVerified,
                    IsPrimary = x.IsPrimary,
                })
                .OrderBy(query.OrderBy);

            return Task.FromResult(projection);
        }
    }
}
