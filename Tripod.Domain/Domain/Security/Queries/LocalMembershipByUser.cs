using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Find a LocalMembership by User Id, User Name, or UserLoginInfo.
    /// </summary>
    public class LocalMembershipByUser : BaseEntityQuery<LocalMembership>, IDefineQuery<Task<LocalMembership>>
    {
        /// <summary>
        /// Find a LocalMembership by User Id.
        /// </summary>
        /// <param name="userId">Id of the User to find a LocalMembership for.</param>
        public LocalMembershipByUser(int userId) { UserId = userId; }

        /// <summary>
        /// Find a LocalMembership by User Name.
        /// </summary>
        /// <param name="userName">Name of the User to find a LocalMembership for.</param>
        public LocalMembershipByUser(string userName) { UserName = userName; }

        /// <summary>
        /// Find a LocalMembership by UserLoginInfo (login provider and provider key)
        /// </summary>
        /// <param name="userLoginInfo">Login provider and key belonging to the User to find
        ///     a LocalMembership for.</param>
        public LocalMembershipByUser(UserLoginInfo userLoginInfo) { UserLoginInfo = userLoginInfo; }

        public int? UserId { get; private set; }
        public string UserName { get; private set; }
        public UserLoginInfo UserLoginInfo { get; private set; }
    }

    [UsedImplicitly]
    public class HandleLocalMembershipByUserQuery : IHandleQuery<LocalMembershipByUser, Task<LocalMembership>>
    {
        private readonly IReadEntities _entities;

        public HandleLocalMembershipByUserQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public async Task<LocalMembership> Handle(LocalMembershipByUser query)
        {
            var queryable = _entities.Query<LocalMembership>().EagerLoad(query.EagerLoad);
            Task<LocalMembership> entityTask;

            if (query.UserId.HasValue)
                entityTask = queryable.ByUserIdAsync(query.UserId.Value);
            else if (query.UserLoginInfo != null)
                entityTask = queryable.ByUserLoginInfoAsync(query.UserLoginInfo);
            else
                entityTask = queryable.ByUserNameAsync(query.UserName);

            return await entityTask.ConfigureAwait(false);
        }
    }
}
