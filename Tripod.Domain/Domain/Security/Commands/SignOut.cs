using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class SignOut : IDefineCommand { }

    [UsedImplicitly]
    public class HandleSignOutCommand : IHandleCommand<SignOut>
    {
        private readonly IAuthenticate _authenticator;

        public HandleSignOutCommand(IAuthenticate authenticator)
        {
            _authenticator = authenticator;
        }

        public async Task Handle(SignOut command)
        {
            await _authenticator.SignOut();
        }
    }
}
