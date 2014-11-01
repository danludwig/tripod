using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Find a RemoteMembership by UserLoginInfo and either User Id or User Name.
    /// </summary>
    public class RemoteMembershipBy : BaseEntityQuery<RemoteMembership>, IDefineQuery<Task<RemoteMembership>>
    {
        /// <summary>
        /// Find a RemoteMembership by User Id and UserLoginInfo.
        /// </summary>
        /// <param name="userLoginInfo">Login provider and key of the User to find.</param>
        /// <param name="userId">Id of the User expected to own the RemoteMembership.</param>
        public RemoteMembershipBy(UserLoginInfo userLoginInfo, int? userId = null)
        {
            UserLoginInfo = userLoginInfo;
            UserId = userId;
        }

        /// <summary>
        /// Find a RemoteMembership by User Name and UserLoginInfo.
        /// </summary>
        /// <param name="userLoginInfo">Login provider and key of the User to find.</param>
        /// <param name="userName">Name of the User expected to own the RemoteMembership.</param>
        public RemoteMembershipBy(UserLoginInfo userLoginInfo, string userName)
        {
            UserLoginInfo = userLoginInfo;
            UserName = userName;
        }

        public UserLoginInfo UserLoginInfo { get; private set; }
        public int? UserId { get; private set; }
        public string UserName { get; private set; }
    }

    [UsedImplicitly]
    public class HandleRemoteMembershipByQuery : IHandleQuery<RemoteMembershipBy, Task<RemoteMembership>>
    {
        private readonly IReadEntities _entities;

        public HandleRemoteMembershipByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<RemoteMembership> Handle(RemoteMembershipBy query)
        {
            if (query.UserLoginInfo == null) return Task.FromResult(null as RemoteMembership);

            var queryable = _entities.Query<RemoteMembership>().EagerLoad(query.EagerLoad);

            if (query.UserId.HasValue)
                return queryable.ByUserIdAndLoginInfoAsync(query.UserId.Value, query.UserLoginInfo);

            if (query.UserName != null)
                return queryable.ByUserNameAndLoginInfoAsync(query.UserName, query.UserLoginInfo);

            return queryable.ByUserLoginInfoAsync(query.UserLoginInfo);
        }
    }
}
