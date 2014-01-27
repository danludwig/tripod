using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class HashedEmailAddress : IDefineQuery<Task<string>>
    {
        public HashedEmailAddress(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public string EmailAddress { get; private set; }
    }

    [UsedImplicitly]
    public class HandleHashedEmailAddressQuery : IHandleQuery<HashedEmailAddress, Task<string>>
    {
        public Task<string> Handle(HashedEmailAddress query)
        {
            // https://en.gravatar.com/site/implement/hash/
            var emailAddress = query.EmailAddress != null ? query.EmailAddress.Trim().ToLower() : "";
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(emailAddress);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var t in hash) 
                sb.Append(t.ToString("x2"));
            var gravatar = sb.ToString();
            return Task.FromResult(gravatar);
        }
    }
}
