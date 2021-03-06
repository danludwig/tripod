﻿using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Generate a hash for an EmailAddress (for use with Gravatar).
    /// </summary>
    public class HashedEmailValueBy : IDefineQuery<Task<string>>
    {
        /// <summary>
        /// Generate a hash for an EmailAddress (for use with Gravatar).
        /// </summary>
        /// <param name="emailAddress">Email address to generate a hash for.</param>
        internal HashedEmailValueBy(string emailAddress) { EmailAddress = emailAddress; }

        internal string EmailAddress { get; private set; }
    }

    [UsedImplicitly]
    public class HandleHashedEmailValueByQuery : IHandleQuery<HashedEmailValueBy, Task<string>>
    {
        public Task<string> Handle(HashedEmailValueBy query)
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
