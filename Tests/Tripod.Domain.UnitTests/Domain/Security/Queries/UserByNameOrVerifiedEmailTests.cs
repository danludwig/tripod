using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class UserByNameOrVerifiedEmailTests
    {
        [Fact]
        public void Query_Ctor_SetsNameOrEmailProperty()
        {
            var nameOrEmail = FakeData.Email();
            var query = new UserByNameOrVerifiedEmail(nameOrEmail);
            query.NameOrEmail.ShouldEqual(nameOrEmail);
        }

        [Fact]
        public void Handler_ReturnsNullUser_WhenNotFound()
        {
            var nameOrEmail = FakeData.Email();
            var query = new UserByNameOrVerifiedEmail(nameOrEmail);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserBy, bool>> expectedUserQuery =
                x => x.Name == nameOrEmail;
            queries.Setup(x => x.Execute(It.Is(expectedUserQuery)))
                .Returns(Task.FromResult(null as User));
            Expression<Func<EmailAddressBy, bool>> expectedEmailQuery =
                x => x.Value == nameOrEmail && x.IsVerified == true;
            queries.Setup(x => x.Execute(It.Is(expectedEmailQuery)))
                .Returns(Task.FromResult(null as EmailAddress));
            var handler = new HandleUserByNameOrVerifiedEmailQuery(queries.Object);

            User result = handler.Handle(query).Result;

            result.ShouldBeNull();
            queries.Verify(x => x.Execute(It.Is(expectedUserQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(expectedEmailQuery)), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullUser_WhenFound_ByUnverifiedEmail()
        {
            var nameOrEmail = FakeData.Email();
            var query = new UserByNameOrVerifiedEmail(nameOrEmail);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserBy, bool>> expectedUserQuery =
                x => x.Name == nameOrEmail;
            queries.Setup(x => x.Execute(It.Is(expectedUserQuery)))
                .Returns(Task.FromResult(null as User));
            var user = new User { Name = Guid.NewGuid().ToString(), };
            var emailAddress = new EmailAddress
            {
                Value = nameOrEmail,
                UserId = user.Id,
                User = user,
                IsVerified = false,
            };
            Expression<Func<EmailAddressBy, bool>> expectedEmailQuery =
                x => x.Value == nameOrEmail && x.IsVerified == true;
            queries.Setup(x => x.Execute(It.Is(expectedEmailQuery)))
                .Returns(Task.FromResult(emailAddress));
            var handler = new HandleUserByNameOrVerifiedEmailQuery(queries.Object);

            User result = handler.Handle(query).Result;

            result.ShouldBeNull();
            queries.Verify(x => x.Execute(It.Is(expectedUserQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(expectedEmailQuery)), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNonNullUser_WhenFound_ByUserName()
        {
            var nameOrEmail = FakeData.Email();
            var query = new UserByNameOrVerifiedEmail(nameOrEmail);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var user = new User { Name = nameOrEmail, };
            Expression<Func<UserBy, bool>> expectedUserQuery =
                x => x.Name == nameOrEmail;
            queries.Setup(x => x.Execute(It.Is(expectedUserQuery)))
                .Returns(Task.FromResult(user));
            queries.Setup(x => x.Execute(It.IsAny<EmailAddressBy>()))
                .Returns(Task.FromResult(null as EmailAddress));
            var handler = new HandleUserByNameOrVerifiedEmailQuery(queries.Object);

            User result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.Name.ShouldEqual(nameOrEmail);
            queries.Verify(x => x.Execute(It.Is(expectedUserQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressBy>()), Times.Never);
        }

        [Fact]
        public void Handler_ReturnsNonNullUser_WhenFound_ByVerifiedEmail()
        {
            var nameOrEmail = FakeData.Email();
            var query = new UserByNameOrVerifiedEmail(nameOrEmail);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserBy, bool>> expectedUserQuery =
                x => x.Name == nameOrEmail;
            queries.Setup(x => x.Execute(It.Is(expectedUserQuery)))
                .Returns(Task.FromResult(null as User));
            var user = new User { Name = Guid.NewGuid().ToString(), };
            var emailAddress = new EmailAddress
            {
                Value = nameOrEmail,
                UserId = user.Id,
                User = user,
                IsVerified = true,
            };
            Expression<Func<EmailAddressBy, bool>> expectedEmailQuery =
                x => x.Value == nameOrEmail && x.IsVerified == true;
            queries.Setup(x => x.Execute(It.Is(expectedEmailQuery)))
                .Returns(Task.FromResult(emailAddress));
            var handler = new HandleUserByNameOrVerifiedEmailQuery(queries.Object);

            User result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.Name.ShouldEqual(user.Name);
            queries.Verify(x => x.Execute(It.Is(expectedUserQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(expectedEmailQuery)), Times.Once);
        }
    }
}
