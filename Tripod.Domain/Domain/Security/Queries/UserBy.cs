using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class UserBy : BaseEntityQuery<User>, IDefineQuery<Task<User>>
    {
        public UserBy(int id) { Id = id; }
        public UserBy(string name) { Name = name; }
        public UserBy(UserLoginInfo userLoginInfo) { UserLoginInfo = userLoginInfo; }
        public UserBy(IPrincipal principal) { Principal = principal; }

        public int? Id { get; private set; }
        public string Name { get; private set; }
        public UserLoginInfo UserLoginInfo { get; private set; }
        public IPrincipal Principal { get; private set; }
    }

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
