using System.Linq;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Find all RemoteMemberships by User Id or User Name.
    /// </summary>
    public class RemoteMembershipsByUser : BaseEntitiesQuery<RemoteMembership>, IDefineQuery<Task<IQueryable<RemoteMembership>>>
    {
        /// <summary>
        /// Find all RemoteMemberships by User Id.
        /// </summary>
        /// <param name="userId">Id of the User to find RemoteMemberships for.</param>
        public RemoteMembershipsByUser(int userId) { UserId = userId; }

        /// <summary>
        /// Find all Remote Memberships by User Name.
        /// </summary>
        /// <param name="userName"></param>
        public RemoteMembershipsByUser(string userName) { UserName = userName; }

        public int? UserId { get; private set; }
        public string UserName { get; private set; }
    }

    [UsedImplicitly]
    public class HandleRemoteMembershipsByUserQuery : IHandleQuery<RemoteMembershipsByUser, Task<IQueryable<RemoteMembership>>>
    {
        private readonly IReadEntities _entities;

        public HandleRemoteMembershipsByUserQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<IQueryable<RemoteMembership>> Handle(RemoteMembershipsByUser query)
        {
            var queryable = _entities.Query<RemoteMembership>().EagerLoad(query.EagerLoad);
            if (query.UserId.HasValue) return Task.FromResult(queryable.ByUserId(query.UserId.Value));
            return Task.FromResult(queryable.ByUserName(query.UserName));
        }
    }
}
