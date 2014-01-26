using System;
using System.Linq.Expressions;
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
            var user = new User { Name = command.UserName };
            var userResult = Task.FromResult(user);
            var userStore = new Mock<IUserStore<User, int>>();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var userManager = new Mock<UserManager<User, int>>(userStore.Object);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            var handler = new HandleSignInCommand(queries.Object, userManager.Object, authenticator.Object);
            Expression<Func<UserByNameOrConfirmedEmail, bool>> userByNameOrConfirmedEmail = x => x.NameOrEmail == command.UserName;
            queries.Setup(x => x.Execute(It.Is(userByNameOrConfirmedEmail))).Returns(userResult);
            userManager.Setup(x => x.FindAsync(command.UserName, command.Password))
                .Returns(userResult);
            authenticator.Setup(x => x.SignOn(It.IsAny<User>(), It.IsAny<bool>())).Returns(Task.FromResult(0));

            handler.Handle(command).Wait();

            userManager.Verify(x => x.FindAsync(command.UserName, command.Password), Times.Once);
        }

        [Fact]
        public void AuthenticatesUser()
        {
            var command = new SignIn { UserName = "username", Password = "password" };
            var user = new User { Name = command.UserName };
            var userResult = Task.FromResult(user);
            var userStore = new Mock<IUserStore<User, int>>();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var userManager = new Mock<UserManager<User, int>>(userStore.Object);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            var handler = new HandleSignInCommand(queries.Object, userManager.Object, authenticator.Object);
            Expression<Func<UserByNameOrConfirmedEmail, bool>> userByNameOrConfirmedEmail = x => x.NameOrEmail == command.UserName;
            queries.Setup(x => x.Execute(It.Is(userByNameOrConfirmedEmail))).Returns(userResult);
            userManager.Setup(x => x.FindAsync(command.UserName, command.Password)).Returns(userResult);
            authenticator.Setup(x => x.SignOn(user, command.IsPersistent)).Returns(Task.FromResult(0));

            handler.Handle(command).Wait();

            authenticator.Verify(x => x.SignOn(user, command.IsPersistent), Times.Once);
        }

        [Theory, InlineData(true), InlineData(false)]
        public void AuthenticatesUser_UsingCommand_IsPersistent(bool isPersistent)
        {
            var command = new SignIn { UserName = "username", Password = "password", IsPersistent = isPersistent };
            var user = new User { Name = command.UserName };
            var userResult = Task.FromResult(user);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var userStore = new Mock<IUserStore<User, int>>();
            var userManager = new Mock<UserManager<User, int>>(userStore.Object);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            var handler = new HandleSignInCommand(queries.Object, userManager.Object, authenticator.Object);
            Expression<Func<UserByNameOrConfirmedEmail, bool>> userByNameOrConfirmedEmail = x => x.NameOrEmail == command.UserName;
            queries.Setup(x => x.Execute(It.Is(userByNameOrConfirmedEmail))).Returns(userResult);
            userManager.Setup(x => x.FindAsync(command.UserName, command.Password)).Returns(userResult);
            authenticator.Setup(x => x.SignOn(user, isPersistent)).Returns(Task.FromResult(0));

            handler.Handle(command).Wait();

            authenticator.Verify(x => x.SignOn(user, isPersistent), Times.Once);
        }
    }
}
