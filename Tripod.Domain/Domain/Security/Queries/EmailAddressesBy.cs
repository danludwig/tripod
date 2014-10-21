using System.Linq;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Find all EmailAddreses by User Id.
    /// </summary>
    public class EmailAddressesBy : BaseEntitiesQuery<EmailAddress>, IDefineQuery<Task<IQueryable<EmailAddress>>>
    {
        /// <summary>
        /// Find all EmailAddreses by User Id.
        /// </summary>
        /// <param name="userId">Id of the User to find EmailAddresses for.</param>
        public EmailAddressesBy(int userId) { UserId = userId; }

        public int UserId { get; private set; }

        /// <summary>
        /// When not null, the EmailAddress IsVerified property must match this value.
        /// </summary>
        public bool? IsVerified { get; set; }
    }

    [UsedImplicitly]
    public class HandleEmailAddressesByQuery : IHandleQuery<EmailAddressesBy, Task<IQueryable<EmailAddress>>>
    {
        private readonly IReadEntities _entities;

        public HandleEmailAddressesByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public Task<IQueryable<EmailAddress>> Handle(EmailAddressesBy query)
        {
            var queryable = _entities.Query<EmailAddress>()
                .EagerLoad(query.EagerLoad)
                .ByUserId(query.UserId);

            if (query.IsVerified.HasValue)
                queryable = queryable.Where(x => x.IsVerified == query.IsVerified.Value);

            return Task.FromResult(queryable);
        }
    }
}
