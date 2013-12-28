using System;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class UserBy : BaseEntityQuery<User>, IDefineQuery<Task<User>>
    {
        public UserBy(int id) { Id = id; }
        public UserBy(string name) { Name = name; }

        public int? Id { get; private set; }
        public string Name { get; private set; }
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
            var entity = query.Id.HasValue
                ? queryable.ByIdAsync(query.Id.Value)
                : queryable.ByNameAsync(query.Name);

            return entity;
        }
    }
}
