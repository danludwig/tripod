using System.Data.Entity;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class UserHasLocalMembership : IDefineQuery<Task<bool>>
    {
        [UsedImplicitly]
        public UserHasLocalMembership(int userId)
        {
            UserId = userId;
        }

        public UserHasLocalMembership(string userName)
        {
            UserName = userName;
        }

        public UserHasLocalMembership(IPrincipal principal)
        {
            Principal = principal;
        }

        public int? UserId { get; private set; }
        public string UserName { get; private set; }
        public IPrincipal Principal { get; private set; }
    }

    [UsedImplicitly]
    public class HandleUserHasLocalMembershipQuery : IHandleQuery<UserHasLocalMembership, Task<bool>>
    {
        private readonly IReadEntities _entities;

        public HandleUserHasLocalMembershipQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<bool> Handle(UserHasLocalMembership query)
        {
            var queryable = _entities.Query<LocalMembership>();

            if (query.UserId.HasValue) return queryable.AnyAsync(QueryLocalMemberships.ByUserId(query.UserId.Value));

            if (query.Principal != null && query.Principal.Identity.HasAppUserId())
                return queryable.AnyAsync(QueryLocalMemberships.ByUserId(query.Principal.Identity.GetAppUserId()));

            return queryable.AnyAsync(QueryLocalMemberships.ByUserName(query.UserName));
        }
    }
}
