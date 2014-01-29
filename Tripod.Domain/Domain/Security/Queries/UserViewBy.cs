using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class UserViewBy : IDefineQuery<Task<UserView>>
    {
        public UserViewBy(int id) { Id = id; }
        public UserViewBy(string name) { Name = name; }
        public UserViewBy(IPrincipal principal) { Principal = principal; }

        public int? Id { get; private set; }
        public string Name { get; private set; }
        public IPrincipal Principal { get; private set; }
    }

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
            {
                queryable = queryable.Where(EntityExtensions.ById<User>(query.Id.Value));
            }
            else if (query.Principal != null && query.Principal.Identity.IsAuthenticated)
            {
                queryable = queryable.Where(EntityExtensions.ById<User>(query.Principal.Identity.GetAppUserId()));
            }
            else
            {
                queryable = queryable.Where(QueryUsers.ByName(query.Name));
            }

            var projection = await queryable.Select(x => new
            {
                UserId = x.Id,
                UserName = x.Name,
                DefaultEmailAddress = x.EmailAddresses.Where(y => y.IsDefault)
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
                DefaultEmailAddress = projection.DefaultEmailAddress.Value,
                DefaultEmailHash = projection.DefaultEmailAddress.HashedValue,
            };

            return view;
        }
    }
}
