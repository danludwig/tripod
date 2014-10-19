using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class IsPasswordVerifiedTests
    {
        [Fact]
        public void Handler_ReturnsFalse_WhenNoUserExists_WithNameOrVerifiedEmail()
        {
            string nameOrVerifiedEmail = string.Format("{0}@domain.tld", Guid.NewGuid());
            string password = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserByNameOrVerifiedEmail, bool>> expectedQuery =
                x => x.NameOrEmail == nameOrVerifiedEmail;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(null as User));
            var userStore = new Mock<IUserStore<User, int>>(MockBehavior.Strict);
            var userManager = new UserManager<User, int>(userStore.Object);
            var handler = new HandleIsPasswordVerifiedQuery(queries.Object, userManager);
            var query = new IsPasswordVerified
            {
                UserNameOrVerifiedEmail = nameOrVerifiedEmail,
                Password = password,
            };

            bool result = handler.Handle(query).Result;

            result.ShouldBeFalse();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once());
        }

        [Fact]
        public void Handler_ReturnsFalse_WhenUserExists_ButHasDifferentPassword()
        {
            string nameOrVerifiedEmail = string.Format("{0}@domain.tld", Guid.NewGuid());
            string password = Guid.NewGuid().ToString();
            var user = new User { Name = nameOrVerifiedEmail, };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserByNameOrVerifiedEmail, bool>> expectedQuery =
                x => x.NameOrEmail == nameOrVerifiedEmail;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(user));
            var userStore = new Mock<IUserStore<User, int>>(MockBehavior.Strict);
            userStore.Setup(x => x.FindByNameAsync(nameOrVerifiedEmail)).Returns(Task.FromResult(user));
            var passwordHasher = new PasswordHasher();
            Expression<Func<User, bool>> expectedUser = x => x.Name == nameOrVerifiedEmail;
            userStore.As<IUserPasswordStore<User, int>>().Setup(x => x.GetPasswordHashAsync(It.Is(expectedUser)))
                .Returns(Task.FromResult(passwordHasher.HashPassword(Guid.NewGuid().ToString())));
            var userManager = new UserManager<User, int>(userStore.Object);
            var handler = new HandleIsPasswordVerifiedQuery(queries.Object, userManager);
            var query = new IsPasswordVerified
            {
                UserNameOrVerifiedEmail = nameOrVerifiedEmail,
                Password = password,
            };

            bool result = handler.Handle(query).Result;

            result.ShouldBeFalse();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once());
            userStore.Verify(x => x.FindByNameAsync(nameOrVerifiedEmail), Times.Once);
            userStore.As<IUserPasswordStore<User, int>>().Verify(
                x => x.GetPasswordHashAsync(It.Is(expectedUser)), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsTrue_WhenUserExists_AndPasswordHashesMatch()
        {
            string nameOrVerifiedEmail = string.Format("{0}@domain.tld", Guid.NewGuid());
            string password = Guid.NewGuid().ToString();
            var user = new User { Name = nameOrVerifiedEmail, };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserByNameOrVerifiedEmail, bool>> expectedQuery =
                x => x.NameOrEmail == nameOrVerifiedEmail;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(user));
            var userStore = new Mock<IUserStore<User, int>>(MockBehavior.Strict);
            userStore.Setup(x => x.FindByNameAsync(nameOrVerifiedEmail)).Returns(Task.FromResult(user));
            var passwordHasher = new PasswordHasher();
            Expression<Func<User, bool>> expectedUser = x => x.Name == nameOrVerifiedEmail;
            userStore.As<IUserPasswordStore<User, int>>().Setup(x => x.GetPasswordHashAsync(It.Is(expectedUser)))
                .Returns(Task.FromResult(passwordHasher.HashPassword(password)));
            var userManager = new UserManager<User, int>(userStore.Object);
            var handler = new HandleIsPasswordVerifiedQuery(queries.Object, userManager);
            var query = new IsPasswordVerified
            {
                UserNameOrVerifiedEmail = nameOrVerifiedEmail,
                Password = password,
            };

            bool result = handler.Handle(query).Result;

            result.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once());
            userStore.Verify(x => x.FindByNameAsync(nameOrVerifiedEmail), Times.Once);
            userStore.As<IUserPasswordStore<User, int>>().Verify(
                x => x.GetPasswordHashAsync(It.Is(expectedUser)), Times.Once);
        }
    }
}
