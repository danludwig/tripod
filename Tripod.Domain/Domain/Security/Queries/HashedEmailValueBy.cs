using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class HashedEmailValueBy : IDefineQuery<Task<string>>
    {
        public HashedEmailValueBy(int userId)
        {
            UserId = userId;
        }

        internal HashedEmailValueBy(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public int? UserId { get; set; }
        internal string EmailAddress { get; private set; }
    }

    [UsedImplicitly]
    public class HandleHashedEmailValueByQuery : IHandleQuery<HashedEmailValueBy, Task<string>>
    {
        private readonly IReadEntities _entities;

        public HandleHashedEmailValueByQuery(IReadEntities entities)
        {
            _entities = entities;
        }

        public async Task<string> Handle(HashedEmailValueBy query)
        {
            // todo: should probably put the gravatar hash in a cookie to avoid hitting storage on every request
            if (query.UserId.HasValue)
            {
                var emailHash = await _entities.Query<EmailAddress>()
                    .Where(x => x.OwnerId == query.UserId && x.IsConfirmed && x.IsDefault)
                    .Select(x => x.HashedValue)
                    .SingleOrDefaultAsync().ConfigureAwait(false);
                return emailHash ?? "0";
            }

            // https://en.gravatar.com/site/implement/hash/
            var emailAddress = query.EmailAddress != null ? query.EmailAddress.Trim().ToLower() : "";
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(emailAddress);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var t in hash) 
                sb.Append(t.ToString("x2"));
            var gravatar = sb.ToString();
            return gravatar;
        }
    }
}
