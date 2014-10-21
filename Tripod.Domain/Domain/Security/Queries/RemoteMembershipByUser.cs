using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Find a RemoteMembership by UserLoginInfo and either User Id or User Name.
    /// </summary>
    public class RemoteMembershipByUser : BaseEntityQuery<RemoteMembership>, IDefineQuery<Task<RemoteMembership>>
    {
        /// <summary>
        /// Find a RemoteMembership by User Id and UserLoginInfo.
        /// </summary>
        /// <param name="userId">Id of the User expected to own the RemoteMembership.</param>
        /// <param name="userLoginInfo">Login provider and key of the User to find.</param>
        public RemoteMembershipByUser(int userId, UserLoginInfo userLoginInfo)
        {
            UserId = userId;
            UserLoginInfo = userLoginInfo;
        }

        /// <summary>
        /// Find a RemoteMembership by User Name and UserLoginInfo.
        /// </summary>
        /// <param name="userName">Name of the User expected to own the RemoteMembership.</param>
        /// <param name="userLoginInfo">Login provider and key of the User to find.</param>
        public RemoteMembershipByUser(string userName, UserLoginInfo userLoginInfo)
        {
            UserName = userName;
            UserLoginInfo = userLoginInfo;
        }

        public int? UserId { get; private set; }
        public string UserName { get; private set; }
        public UserLoginInfo UserLoginInfo { get; private set; }
    }

    [UsedImplicitly]
    public class HandleRemoteMembershipByUserQuery : IHandleQuery<RemoteMembershipByUser, Task<RemoteMembership>>
    {
        private readonly IReadEntities _entities;

        public HandleRemoteMembershipByUserQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<RemoteMembership> Handle(RemoteMembershipByUser query)
        {
            if (query.UserLoginInfo == null) return Task.FromResult(null as RemoteMembership);

            var queryable = _entities.Query<RemoteMembership>().EagerLoad(query.EagerLoad);
            if (query.UserId.HasValue) return queryable.ByUserIdAndLoginInfoAsync(query.UserId.Value, query.UserLoginInfo);
            return queryable.ByUserNameAndLoginInfoAsync(query.UserName, query.UserLoginInfo);
        }
    }
}
