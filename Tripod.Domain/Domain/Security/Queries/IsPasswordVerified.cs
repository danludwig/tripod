using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class IsPasswordVerified : IDefineQuery<Task<bool>>
    {
        internal IsPasswordVerified() { }
        internal string UserNameOrVerifiedEmail { get; set; }
        internal string Password { get; set; }
    }

    [UsedImplicitly]
    public class HandleIsPasswordVerifiedQuery : IHandleQuery<IsPasswordVerified, Task<bool>>
    {
        private readonly IProcessQueries _queries;
        private readonly UserManager<User, int> _userManager;

        public HandleIsPasswordVerifiedQuery(IProcessQueries queries, UserManager<User, int> userManager)
        {
            _queries = queries;
            _userManager = userManager;
        }

        public async Task<bool> Handle(IsPasswordVerified query)
        {
            // match password with either a username or a verified email address
            var user = await _queries.Execute(new UserByNameOrVerifiedEmail(query.UserNameOrVerifiedEmail))
                .ConfigureAwait(false);
            if (user == null) return false;

            var entity = await _userManager.FindAsync(user.Name, query.Password)
                .ConfigureAwait(false);
            return entity != null;
        }
    }
}
