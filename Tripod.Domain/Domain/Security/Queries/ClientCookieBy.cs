using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class ClientCookieBy : IDefineQuery<Task<ClientCookie>>
    {
        public ClientCookieBy(int userId)
        {
            UserId = userId;
        }

        public int? UserId { get; private set; }
    }

    public class HandleClientCookieByQuery : IHandleQuery<ClientCookieBy, Task<ClientCookie>>
    {
        private readonly IReadEntities _entities;

        public HandleClientCookieByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public async Task<ClientCookie> Handle(ClientCookieBy query)
        {
            var queryable = _entities.Query<User>();

            if (query.UserId.HasValue)
            {
                queryable = queryable.Where(EntityExtensions.ById<User>(query.UserId.Value));
                var clientCookie = await queryable.Select(x => new ClientCookie
                {
                    UserId = x.Id,
                    UserName = x.Name,
                    GravatarHash = x.EmailAddresses.FirstOrDefault(y => y.IsPrimary).HashedValue,
                })
                .SingleOrDefaultAsync().ConfigureAwait(false);
                return clientCookie;
            }

            return null;
        }
    }
}
