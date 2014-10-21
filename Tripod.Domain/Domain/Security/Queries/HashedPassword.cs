using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Generate a hash for a password.
    /// </summary>
    public class HashedPassword : IDefineQuery<Task<string>>
    {
        /// <summary>
        /// Generate a hash for a password.
        /// </summary>
        /// <param name="password">The password to generate a hash for.</param>
        public HashedPassword(string password) { Password = password; }

        public string Password { get; private set; }
    }

    [UsedImplicitly]
    public class HandleHashedPasswordQuery : IHandleQuery<HashedPassword, Task<string>>
    {
        private readonly UserManager<User, int> _userManager;

        public HandleHashedPasswordQuery(UserManager<User, int> userManager)
        {
            _userManager = userManager;
        }

        public Task<string> Handle(HashedPassword query)
        {
            var hashedPassword = _userManager.PasswordHasher.HashPassword(query.Password);
            return Task.FromResult(hashedPassword);
        }
    }
}
