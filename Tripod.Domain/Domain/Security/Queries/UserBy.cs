using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class UserBy : BaseEntityQuery<User>, IDefineQuery<Task<User>>
    {
        public UserBy(int id) { Id = id; }
        public UserBy(string name) { Name = name; }
        public UserBy(UserLoginInfo userLoginInfo) { UserLoginInfo = userLoginInfo; }

        public int? Id { get; private set; }
        public string Name { get; private set; }
        public UserLoginInfo UserLoginInfo { get; private set; }
    }

    public class HandleUserByQuery : IHandleQuery<UserBy, Task<User>>
    {
        private readonly IReadEntities _entities;

        public HandleUserByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<User> Handle(UserBy query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var queryable = _entities.Query<User>().EagerLoad(query.EagerLoad);
            if (query.Id.HasValue) return queryable.ByIdAsync(query.Id.Value);
            if (query.UserLoginInfo != null) return queryable.ByUserLoginInfoAsync(query.UserLoginInfo);
            return queryable.ByNameAsync(query.Name);
        }
    }
}
