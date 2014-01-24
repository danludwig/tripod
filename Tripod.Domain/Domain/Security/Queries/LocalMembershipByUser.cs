using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class LocalMembershipByUser : BaseEntityQuery<LocalMembership>, IDefineQuery<Task<LocalMembership>>
    {
        public LocalMembershipByUser(int userId) { UserId = userId; }
        public LocalMembershipByUser(string userName) { UserName = userName; }
        public LocalMembershipByUser(UserLoginInfo userLoginInfo) { UserLoginInfo = userLoginInfo; }

        public int? UserId { get; private set; }
        public string UserName { get; private set; }
        public UserLoginInfo UserLoginInfo { get; private set; }
    }

    public class HandleLocalMembershipByUserQuery : IHandleQuery<LocalMembershipByUser, Task<LocalMembership>>
    {
        private readonly IReadEntities _entities;

        public HandleLocalMembershipByUserQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<LocalMembership> Handle(LocalMembershipByUser query)
        {
            var queryable = _entities.Query<LocalMembership>().EagerLoad(query.EagerLoad);
            if (query.UserId.HasValue) return queryable.ByUserIdAsync(query.UserId.Value);
            if (query.UserLoginInfo != null) return queryable.ByUserLoginInfoAsync(query.UserLoginInfo);
            return queryable.ByUserNameAsync(query.UserName);
        }
    }
}
