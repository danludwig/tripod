using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class EmailAddressesByTests
    {
        #region EmailAddressesBy UserId

        [Fact]
        public void Query_IntCtor_SetsUserIdProperty()
        {
            var userId = FakeData.Id();
            var query = new EmailAddressesBy(userId);
            query.UserId.ShouldEqual(userId);
        }

        [Fact]
        public void Handler_ReturnsNoEmailAddresses_WhenNotFound_ByUserId()
        {
            var userId = FakeData.Id();
            var otherUserId1 = FakeData.Id(canNotBe: userId);
            var otherUserId2 = FakeData.Id(userId, otherUserId1);
            var data = new[]
            {
                new EmailAddress { UserId = otherUserId1, IsPrimary = true, },
                new EmailAddress { UserId = otherUserId1, },
                new EmailAddress { UserId = otherUserId1, },
                new EmailAddress { UserId = otherUserId2, IsPrimary = true, },
                new EmailAddress { UserId = otherUserId2, },
            }.AsQueryable();
            var query = new EmailAddressesBy(userId);
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressesByQuery(entities.Object);

            EmailAddress[] result = handler.Handle(query).Result.ToArray();

            Assert.NotNull(result);
            result.Length.ShouldEqual(0);
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handler_ReturnsNoEmailAddresses_WhenFound_ByUserId_ButIsVerifiedDoesNotMatch(bool isVerified)
        {
            var userId = FakeData.Id();
            var data = new[]
            {
                new EmailAddress { UserId = userId - 2, IsPrimary = true, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 1, IsPrimary = true, },
                new EmailAddress { UserId = userId - 1, },
                new EmailAddress { UserId = userId, IsVerified = !isVerified, },
            }.AsQueryable();
            var query = new EmailAddressesBy(userId)
            {
                IsVerified = isVerified
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressesByQuery(entities.Object);

            EmailAddress[] result = handler.Handle(query).Result.ToArray();

            Assert.NotNull(result);
            result.Length.ShouldEqual(0);
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(null, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Handler_ReturnsEmailAddresses_WhenFound_ByUserId_AndIsVerifiedMatches(
            bool? queryIsVerified, bool entityIsVerified)
        {
            var userId = FakeData.Id();
            var otherUserId1 = FakeData.Id(canNotBe: userId);
            var otherUserId2 = FakeData.Id(userId, otherUserId1);
            var data = new[]
            {
                new EmailAddress { UserId = otherUserId1, IsPrimary = true, },
                new EmailAddress { UserId = otherUserId1, },
                new EmailAddress { UserId = otherUserId1, },
                new EmailAddress { UserId = otherUserId2, IsPrimary = true, },
                new EmailAddress { UserId = otherUserId2, },
                new EmailAddress { UserId = userId, IsVerified = entityIsVerified, },
                new EmailAddress { UserId = userId, IsVerified = !entityIsVerified, },
            }.AsQueryable();
            var query = new EmailAddressesBy(userId)
            {
                IsVerified = queryIsVerified,
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressesByQuery(entities.Object);

            EmailAddress[] result = handler.Handle(query).Result.ToArray();

            Assert.NotNull(result);
            result.Length.ShouldEqual(queryIsVerified.HasValue ? 1 : 2);
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
            if (queryIsVerified.HasValue)
            {
                EmailAddress expectedEntity = data.Single(x => x.UserId == userId && x.IsVerified == entityIsVerified);
                result.Single().ShouldEqual(expectedEntity);
            }
        }

        #endregion
    }
}
