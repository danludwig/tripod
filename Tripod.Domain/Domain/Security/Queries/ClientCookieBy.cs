using System.Linq;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Derive client cookie data for a given User Id.
    /// </summary>
    public class ClientCookieBy : IDefineQuery<Task<ClientCookie>>
    {
        /// <summary>
        /// Derive client cookie data for a given User Id.
        /// </summary>
        /// <param name="userId">Id of the User to generate client cookie data for.</param>
        public ClientCookieBy(int? userId) { UserId = userId; }

        public int? UserId { get; private set; }
    }

    [UsedImplicitly]
    public class HandleClientCookieByQuery : IHandleQuery<ClientCookieBy, Task<ClientCookie>>
    {
        private readonly IProcessQueries _queries;

        public HandleClientCookieByQuery(IProcessQueries queries)
        {
            _queries = queries;
        }

        public async Task<ClientCookie> Handle(ClientCookieBy query)
        {
            ClientCookie clientCookie = null;
            if (query.UserId.HasValue)
            {
                var user = await _queries.Execute(new UserBy(query.UserId.Value))
                    .ConfigureAwait(false);
                if (user != null)
                {
                    clientCookie = new ClientCookie
                    {
                        UserId = user.Id,
                        UserName = user.Name,
                        GravatarHash = user.EmailAddresses.First(y => y.IsPrimary).HashedValue,
                    };
                }
            }

            return clientCookie;
        }
    }
}
