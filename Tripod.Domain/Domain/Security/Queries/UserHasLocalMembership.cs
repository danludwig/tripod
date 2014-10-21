using System.Data.Entity;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Find out whether a User has a LocalMembership by User Id, UserName, or IPrincipal.
    /// </summary>
    public class UserHasLocalMembership : IDefineQuery<Task<bool>>
    {
        /// <summary>
        /// Find out whether a User has a LocalMembership by User Id.
        /// </summary>
        /// <param name="userId">Id of the User to check for LocalMembership.</param>
        public UserHasLocalMembership(int userId) { UserId = userId; }

        /// <summary>
        /// Find out whether a User has a LocalMembership by User Id.
        /// </summary>
        /// <param name="userName">Name of the User to check for LocalMembership.</param>
        public UserHasLocalMembership(string userName) { UserName = userName; }

        /// <summary>
        /// Find out whether a User has a LocalMembership by Principal (uses NameIdentifier of ClaimsIdentity).
        /// </summary>
        /// <param name="principal">Principal with ClaimsIdentity and NameIdentifier with User Id.</param>
        public UserHasLocalMembership(IPrincipal principal) { Principal = principal; }

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
                return queryable.AnyAsync(QueryLocalMemberships.ByUserId(query.Principal.Identity.GetUserId<int>()));

            return queryable.AnyAsync(QueryLocalMemberships.ByUserName(query.UserName));
        }
    }
}
