using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class EmailVerificationTokenIsValid : IDefineQuery<Task<bool>>
    {
        public EmailVerificationTokenIsValid(string token, string ticket, EmailVerificationPurpose purpose)
        {
            Token = token;
            Ticket = ticket;
            Purpose = purpose;
        }

        public string Token { get; private set; }
        public string Ticket { get; private set; }
        public EmailVerificationPurpose Purpose { get; private set; }
    }

    [UsedImplicitly]
    public class HandleEmailVerificationTokenIsValidQuery : IHandleQuery<EmailVerificationTokenIsValid, Task<bool>>
    {
        private readonly UserManager<UserTicket, string> _userManager;

        public HandleEmailVerificationTokenIsValidQuery(UserManager<UserTicket, string> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(EmailVerificationTokenIsValid query)
        {
            if (string.IsNullOrWhiteSpace(query.Ticket) || string.IsNullOrWhiteSpace(query.Token))
                return false;

            var userTicket = new UserTicket { UserName = query.Ticket };
            var purposeString = query.Purpose.ToString();
            bool isValid = await _userManager.UserTokenProvider.ValidateAsync(
                purposeString, query.Token, _userManager, userTicket)
                .ConfigureAwait(false);
            return isValid;
        }
    }
}
