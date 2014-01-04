using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class SignInHandlerTests
    {
        [Fact]
        public void FindsUser_ByNameAndPassword()
        {
            var command = new SignIn { UserName = "username", Password = "password" };
            var userStore = new Mock<IUserStore<User, int>>();
            var userManager = new Mock<UserManager<User, int>>(userStore.Object);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Loose);
            var handler = new HandleSignInCommand(userManager.Object, authenticator.Object);
            userManager.Setup(x => x.FindAsync(command.UserName, command.Password))
                .Returns(Task.FromResult(new User()));
            authenticator.Setup(x => x.SignOn(It.IsAny<User>(), It.IsAny<bool>())).Returns(Task.FromResult(0));

            handler.Handle(command).Wait();

            userManager.Verify(x => x.FindAsync(command.UserName, command.Password), Times.Once);
        }

        [Fact]
        public void AuthenticatesUser()
        {
            var user = new User();
            var command = new SignIn { UserName = "username", Password = "password" };
            var userStore = new Mock<IUserStore<User, int>>();
            var userManager = new Mock<UserManager<User, int>>(userStore.Object);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            var handler = new HandleSignInCommand(userManager.Object, authenticator.Object);
            userManager.Setup(x => x.FindAsync(command.UserName, command.Password))
                .Returns(Task.FromResult(user));
            authenticator.Setup(x => x.SignOn(user, command.IsPersistent)).Returns(Task.FromResult(0));

            handler.Handle(command).Wait();

            authenticator.Verify(x => x.SignOn(user, command.IsPersistent), Times.Once);
        }

        [Theory, InlineData(true), InlineData(false)]
        public void AuthenticatesUser_UsingCommand_IsPersistent(bool isPersistent)
        {
            var user = new User();
            var command = new SignIn { UserName = "username", Password = "password", IsPersistent = isPersistent};
            var userStore = new Mock<IUserStore<User, int>>();
            var userManager = new Mock<UserManager<User, int>>(userStore.Object);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            var handler = new HandleSignInCommand(userManager.Object, authenticator.Object);
            userManager.Setup(x => x.FindAsync(command.UserName, command.Password))
                .Returns(Task.FromResult(user));
            authenticator.Setup(x => x.SignOn(user, isPersistent)).Returns(Task.FromResult(0));

            handler.Handle(command).Wait();

            authenticator.Verify(x => x.SignOn(user,isPersistent), Times.Once);
        }
    }
}
