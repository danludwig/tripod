using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class RemoteMembershipByUser : BaseEntityQuery<RemoteMembership>, IDefineQuery<Task<RemoteMembership>>
    {
        public RemoteMembershipByUser(int userId, UserLoginInfo userLoginInfo)
        {
            UserId = userId;
            UserLoginInfo = userLoginInfo;
        }

        public RemoteMembershipByUser(string userName, UserLoginInfo userLoginInfo)
        {
            UserName = userName;
            UserLoginInfo = userLoginInfo;
        }

        public int? UserId { get; private set; }
        public string UserName { get; private set; }
        public UserLoginInfo UserLoginInfo { get; private set; }
    }

    public class HandleRemoteMembershipByUserQuery : IHandleQuery<RemoteMembershipByUser, Task<RemoteMembership>>
    {
        private readonly IReadEntities _entities;

        public HandleRemoteMembershipByUserQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<RemoteMembership> Handle(RemoteMembershipByUser query)
        {
            if (query == null) throw new ArgumentNullException("query");
            if (query.UserLoginInfo == null) return Task.FromResult(null as RemoteMembership);

            var queryable = _entities.Query<RemoteMembership>().EagerLoad(query.EagerLoad);
            if (query.UserId.HasValue) return queryable.ByUserIdAndLoginInfoAsync(query.UserId.Value, query.UserLoginInfo);
            return queryable.ByUserNameAndLoginInfoAsync(query.UserName, query.UserLoginInfo);
        }
    }
}
