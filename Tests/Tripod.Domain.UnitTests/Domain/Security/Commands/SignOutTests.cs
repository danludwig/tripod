using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Tripod.Domain.Security
{
    public class SignOutTests
    {
        [Fact]
        public void Handle_CallsSignOut_OnAuthenticator()
        {
            var command = new SignOut();
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            authenticator.Setup(x => x.SignOut()).Returns(Task.FromResult(0));
            var handler = new HandleSignOutCommand(authenticator.Object);

            handler.Handle(command).Wait();

            authenticator.Verify(x => x.SignOut(), Times.Once);
        }
    }
}
