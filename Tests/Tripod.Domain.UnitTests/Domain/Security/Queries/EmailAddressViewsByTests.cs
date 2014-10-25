using System;
using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class EmailAddressViewsByTests
    {
        #region EmailAddressViewsBy UserId

        [Fact]
        public void Query_IntCtor_SetsUserIdProperty()
        {
            var userId = new Random().Next(int.MinValue, int.MaxValue);
            var query = new EmailAddressViewsBy(userId);
            query.UserId.ShouldEqual(userId);
        }

        [Fact]
        public void Handler_ReturnsNoEmailAddressViews_WhenNotFound_ByUserId()
        {
            var userId = new Random().Next(3, int.MaxValue);
            var data = new[]
            {
                new EmailAddress { UserId = userId - 2, IsPrimary = true, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 1, IsPrimary = true, },
                new EmailAddress { UserId = userId - 1, },
            }.AsQueryable();
            var query = new EmailAddressViewsBy(userId);
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressViewsByQuery(entities.Object);

            EmailAddressView[] result = handler.Handle(query).Result.ToArray();

            Assert.NotNull(result);
            result.Length.ShouldEqual(0);
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handler_ReturnsNoEmailAddresses_WhenFound_ByUserId_ButIsVerifiedDoesNotMatch(bool isVerified)
        {
            var userId = new Random().Next(1, int.MaxValue);
            var data = new[]
            {
                new EmailAddress { UserId = userId - 2, IsPrimary = true, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 1, IsPrimary = true, },
                new EmailAddress { UserId = userId - 1, },
                new EmailAddress { UserId = userId, IsVerified = !isVerified, },
            }.AsQueryable();
            var query = new EmailAddressViewsBy(userId)
            {
                IsVerified = isVerified
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressViewsByQuery(entities.Object);

            EmailAddressView[] result = handler.Handle(query).Result.ToArray();

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
            var userId = new Random().Next(3, int.MaxValue);
            var data = new[]
            {
                new EmailAddress { UserId = userId - 2, IsPrimary = true, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 1, IsPrimary = true, },
                new EmailAddress { UserId = userId - 1, },
                new ProxiedEmailAddress(664)
                {
                    UserId = userId,
                    Value = FakeData.Email(),
                    HashedValue = "hashed email value",
                    IsVerified = entityIsVerified,
                    IsPrimary = entityIsVerified,
                },
                new EmailAddress { UserId = userId, IsVerified = !entityIsVerified, },
            }.AsQueryable();
            var query = new EmailAddressViewsBy(userId)
            {
                IsVerified = queryIsVerified,
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressViewsByQuery(entities.Object);

            EmailAddressView[] result = handler.Handle(query).Result.ToArray();

            Assert.NotNull(result);
            result.Length.ShouldEqual(queryIsVerified.HasValue ? 1 : 2);
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
            if (queryIsVerified.HasValue)
            {
                EmailAddress expectedEntity = data.Single(x => x.UserId == userId && x.IsVerified == entityIsVerified);
                EmailAddressView actualView = result.Single();
                actualView.EmailAddressId.ShouldEqual(expectedEntity.Id);
                actualView.UserId.ShouldEqual(expectedEntity.UserId);
                actualView.Value.ShouldEqual(expectedEntity.Value);
                actualView.HashedValue.ShouldEqual(expectedEntity.HashedValue);
                actualView.IsPrimary.ShouldEqual(expectedEntity.IsPrimary);
                actualView.IsVerified.ShouldEqual(expectedEntity.IsVerified);
            }
        }

        #endregion
    }
}
