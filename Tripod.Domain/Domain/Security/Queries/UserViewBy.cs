using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Find a UserView by User Id, User Name, or IPrincipal.
    /// </summary>
    public class UserViewBy : IDefineQuery<Task<UserView>>
    {
        /// <summary>
        /// Find a UserView by User Id.
        /// </summary>
        /// <param name="id">Id of the UserView to find.</param>
        public UserViewBy(int id) { Id = id; }

        /// <summary>
        /// Find a UserView by User Name.
        /// </summary>
        /// <param name="name">Name of the UserView to find.</param>
        public UserViewBy(string name) { Name = name; }

        /// <summary>
        /// Find a UserView by Principal (uses NameIdentifier of ClaimsIdentity).
        /// </summary>
        /// <param name="principal">Principal with ClaimsIdentity and NameIdentifier with User Id.</param>
        public UserViewBy(IPrincipal principal) { Principal = principal; }

        public int? Id { get; private set; }
        public string Name { get; private set; }
        public IPrincipal Principal { get; private set; }
    }

    [UsedImplicitly]
    public class HandleUserViewByQuery : IHandleQuery<UserViewBy, Task<UserView>>
    {
        private readonly IReadEntities _entities;

        public HandleUserViewByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public async Task<UserView> Handle(UserViewBy query)
        {
            var queryable = _entities.Query<User>();

            if (query.Id.HasValue)
                queryable = queryable.Where(EntityExtensions.ById<User>(query.Id.Value));

            else if (query.Principal != null && query.Principal.Identity.IsAuthenticated)
                queryable = queryable.Where(EntityExtensions.ById<User>(query.Principal.Identity.GetUserId<int>()));

            else
                queryable = queryable.Where(QueryUsers.ByName(query.Name));

            // project before querying to only get the data needed for the view.
            var projection = await queryable.Select(x => new
            {
                UserId = x.Id,
                UserName = x.Name,
                PrimaryEmailAddress = x.EmailAddresses.Where(y => y.IsPrimary)
                    .Select(y => new
                    {
                        y.Value,
                        y.HashedValue,
                    })
                .FirstOrDefault(),
            })
            .SingleOrDefaultAsync().ConfigureAwait(false);
            if (projection == null) return null;

            var view = new UserView
            {
                UserId = projection.UserId,
                UserName = projection.UserName,
                PrimaryEmailAddress = projection.PrimaryEmailAddress.Value,
                PrimaryEmailHash = projection.PrimaryEmailAddress.HashedValue,
            };

            return view;
        }
    }
}
