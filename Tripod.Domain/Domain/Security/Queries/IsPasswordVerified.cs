using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class IsPasswordVerified : IDefineQuery<Task<bool>>
    {
        internal IsPasswordVerified() { }
        internal string UserName { get; set; }
        internal string Password { get; set; }
    }

    [UsedImplicitly]
    public class HandleIsPasswordVerifiedQuery : IHandleQuery<IsPasswordVerified, Task<bool>>
    {
        private readonly UserManager<User, int> _userManager;

        public HandleIsPasswordVerifiedQuery(UserManager<User, int> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(IsPasswordVerified query)
        {
            var entity = await _userManager.FindAsync(query.UserName, query.Password)
                .ConfigureAwait(false);
            return entity != null;
        }
    }
}
