using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Find a UserView by User Id, User Name, IPrincipal, or UserLoginInfo.
    /// </summary>
    public class UserBy : BaseEntityQuery<User>, IDefineQuery<Task<User>>
    {
        /// <summary>
        /// Find a User by Id.
        /// </summary>
        /// <param name="id">Id of the User to find.</param>
        public UserBy(int id) { Id = id; }

        /// <summary>
        /// Find a User by Name.
        /// </summary>
        /// <param name="name">Name of the User to find.</param>
        public UserBy(string name) { Name = name; }

        /// <summary>
        /// Find a User by UserLoginInfo (login provider and provider key)
        /// </summary>
        /// <param name="userLoginInfo">Login provider and key belonging to the User to find.</param>
        public UserBy(UserLoginInfo userLoginInfo) { UserLoginInfo = userLoginInfo; }

        /// <summary>
        /// Find a User by Principal (uses NameIdentifier of ClaimsIdentity).
        /// </summary>
        /// <param name="principal">Principal with ClaimsIdentity and NameIdentifier with User Id.</param>
        public UserBy(IPrincipal principal) { Principal = principal; }

        public int? Id { get; private set; }
        public string Name { get; private set; }
        public UserLoginInfo UserLoginInfo { get; private set; }
        public IPrincipal Principal { get; private set; }
    }

    [UsedImplicitly]
    public class HandleUserByQuery : IHandleQuery<UserBy, Task<User>>
    {
        private readonly IReadEntities _entities;

        public HandleUserByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public async Task<User> Handle(UserBy query)
        {
            var queryable = _entities.Query<User>().EagerLoad(query.EagerLoad);
            Task<User> entityTask;

            if (query.Id.HasValue)
                entityTask = queryable.ByIdAsync(query.Id.Value);

            else if (query.Principal != null && query.Principal.Identity.IsAuthenticated)
                entityTask = queryable.ByIdAsync(query.Principal.Identity.GetUserId<int>());

            else if (query.UserLoginInfo != null)
                entityTask = queryable.ByUserLoginInfoAsync(query.UserLoginInfo);

            else
                entityTask = queryable.ByNameAsync(query.Name);

            return await entityTask.ConfigureAwait(false);
        }
    }
}
