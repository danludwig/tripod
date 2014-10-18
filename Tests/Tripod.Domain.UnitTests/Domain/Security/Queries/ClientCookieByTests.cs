using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class ClientCookieByTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(43)]
        public void Query_Ctor_SetsUserIdProperty(int? userId)
        {
            var query = new ClientCookieBy(userId);
            query.UserId.ShouldEqual(userId);
        }

        [Fact]
        public void Handler_ReturnsNull_WhenUserId_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var query = new ClientCookieBy(null);
            var handler = new HandleClientCookieByQuery(queries.Object);

            var result = handler.Handle(query).Result;

            result.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNull_WhenUserById_DoesNotExist()
        {
            const int userId = 456;
            var query = new ClientCookieBy(userId);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserBy, bool>> expectedQuery = y => y.Id == query.UserId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as User));
            var handler = new HandleClientCookieByQuery(queries.Object);

            ClientCookie result = handler.Handle(query).Result;

            result.ShouldBeNull();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsClientCookie_WhenUserById_Exists()
        {
            const int userId = 11;
            var userName = Guid.NewGuid().ToString();
            var query = new ClientCookieBy(userId);
            var user = new UserWithSpecifiedId(userId) { Name = userName };
            user.EmailAddresses.Add(new EmailAddress
            {
                HashedValue = "Email 0 hashed value",
            });
            user.EmailAddresses.Add(new EmailAddress
            {
                IsPrimary = true,
                HashedValue = "Email 1 hashed value",
            });
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserBy, bool>> expectedQuery = y => y.Id == query.UserId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(user as User));
            var handler = new HandleClientCookieByQuery(queries.Object);

            ClientCookie result = handler.Handle(query).Result;

            Assert.NotNull(result);
            result.UserId.ShouldEqual(userId);
            result.UserName.ShouldEqual(userName);
            result.GravatarHash.ShouldEqual(user.EmailAddresses.Single(x => x.IsPrimary).HashedValue);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
        }
    }
}
