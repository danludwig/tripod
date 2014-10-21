using System.Security.Claims;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Find an email address by Id, Value, or Claim.
    /// </summary>
    public class EmailAddressBy : BaseEntityQuery<EmailAddress>, IDefineQuery<Task<EmailAddress>>
    {
        /// <summary>
        /// Find an EmailAddress by Id.
        /// </summary>
        /// <param name="id">Id of the EmailAddress to find.</param>
        public EmailAddressBy(int id) { Id = id; }

        /// <summary>
        /// Find an EmailAddress by Value.
        /// </summary>
        /// <param name="value">Value of the EmailAddress to find.</param>
        public EmailAddressBy(string value) { Value = value; }

        /// <summary>
        /// Find an EmailAddress by Claim.
        /// </summary>
        /// <param name="claim">Claim of type Email containing the EmailAddress value.</param>
        public EmailAddressBy(Claim claim) { Claim = claim; }

        public int? Id { get; private set; }
        public string Value { get; private set; }
        public Claim Claim { get; private set; }

        /// <summary>
        /// When not null, the EmailAddress IsVerified property must match this value.
        /// </summary>
        public bool? IsVerified { get; set; }
    }

    [UsedImplicitly]
    public class HandleEmailAddressByQuery : IHandleQuery<EmailAddressBy, Task<EmailAddress>>
    {
        private readonly IReadEntities _entities;

        public HandleEmailAddressByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public async Task<EmailAddress> Handle(EmailAddressBy query)
        {
            var queryable = _entities.Query<EmailAddress>()
                .EagerLoad(query.EagerLoad);

            Task<EmailAddress> entityTask;
            if (query.Id.HasValue)
            {
                entityTask = queryable.ByIdAsync(query.Id.Value);
            }
            else if (query.Claim != null && query.Claim.Type == ClaimTypes.Email)
            {
                entityTask = queryable.ByValueAsync(query.Claim.Value);
            }
            else
            {
                entityTask = queryable.ByValueAsync(query.Value);
            }

            EmailAddress entity = await entityTask.ConfigureAwait(false);

            if (entity != null && query.IsVerified.HasValue && query.IsVerified.Value != entity.IsVerified)
                entity = null;

            return entity;
        }
    }
}
