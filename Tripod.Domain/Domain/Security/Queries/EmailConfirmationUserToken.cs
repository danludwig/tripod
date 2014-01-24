using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class EmailConfirmationUserToken : IDefineQuery<Task<UserToken>>
    {
        public EmailConfirmationUserToken(string token)
        {
            Token = token;
        }

        public string Token { get; private set; }
    }

    [UsedImplicitly]
    public class HandleEmailConfirmationUserTokenQuery : IHandleQuery<EmailConfirmationUserToken, Task<UserToken>>
    {
        private readonly UserManager<User, int> _userManager;

        public HandleEmailConfirmationUserTokenQuery(UserManager<User, int> userManager)
        {
            _userManager = userManager;
        }

        public Task<UserToken> Handle(EmailConfirmationUserToken query)
        {
            var userToken = _userManager.UserConfirmationTokens.Validate(query.Token);
            return Task.FromResult(userToken);
        }
    }
}
