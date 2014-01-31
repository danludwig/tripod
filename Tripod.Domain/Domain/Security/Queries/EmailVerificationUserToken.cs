using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class EmailVerificationUserToken : IDefineQuery<Task<UserToken>>
    {
        public EmailVerificationUserToken(string token)
        {
            Token = token;
        }

        public string Token { get; private set; }
    }

    [UsedImplicitly]
    public class HandleEmailVerificationUserTokenQuery : IHandleQuery<EmailVerificationUserToken, Task<UserToken>>
    {
        private readonly UserManager<User, int> _userManager;

        public HandleEmailVerificationUserTokenQuery(UserManager<User, int> userManager)
        {
            _userManager = userManager;
        }

        public Task<UserToken> Handle(EmailVerificationUserToken query)
        {
            var userToken = _userManager.UserConfirmationTokens.Validate(query.Token);
            return Task.FromResult(userToken);
        }
    }
}
