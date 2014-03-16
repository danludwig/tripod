using System.Linq;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class RemoteMembershipViewsBy : BaseEnumerableQuery<RemoteMembershipView>, IDefineQuery<Task<IQueryable<RemoteMembershipView>>>
    {
        [UsedImplicitly]
        public RemoteMembershipViewsBy(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; private set; }
    }

    [UsedImplicitly]
    public class HandleRemoteMembershipViewsByQuery : IHandleQuery<RemoteMembershipViewsBy, Task<IQueryable<RemoteMembershipView>>>
    {
        private readonly IReadEntities _entities;

        public HandleRemoteMembershipViewsByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<IQueryable<RemoteMembershipView>> Handle(RemoteMembershipViewsBy query)
        {
            var queryable = _entities.Query<RemoteMembership>().ByUserId(query.UserId);

            var projection = queryable
                .Select(x => new RemoteMembershipView
                {
                    Provider = x.LoginProvider,
                    Key = x.ProviderKey,
                    UserId = x.UserId,
                })
                .OrderBy(query.OrderBy);

            return Task.FromResult(projection);
        }
    }
}
