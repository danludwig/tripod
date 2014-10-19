using System;
using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class RemoteMembershipViewsByTests
    {
        #region RemoteMembershipViewsBy Id

        [Fact]
        public void IntCtor_SetsUserIdProperty()
        {
            var userId = new Random().Next(1, int.MaxValue);
            var query = new RemoteMembershipViewsBy(userId);
            query.UserId.ShouldEqual(userId);
        }

        [Fact]
        public void Handle_ReturnsNoRemoteMembershipViews_ByUserId_WhenNotFound()
        {
            var userId = new Random().Next(1, int.MaxValue - 3);
            var remoteMemberships = new[]
            {
                new RemoteMembership { UserId = userId + 1, },
                new RemoteMembership { UserId = userId - 1, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipViewsBy(userId);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipViewsByQuery(entities.Object);

            RemoteMembershipView[] result = handler.Handle(query).Result.ToArray();

            result.ShouldNotBeNull();
            result.Length.ShouldEqual(0);
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Once);
        }

        [Fact]
        public void Handle_ReturnsRemoteMembershipViews_ByUserId_WhenFound()
        {
            var userId = new Random().Next(1, int.MaxValue - 3);
            var remoteMemberships = new[]
            {
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { UserId = userId + 1, },
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { UserId = userId - 1, },
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { UserId = userId, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipViewsBy(userId);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipViewsByQuery(entities.Object);

            RemoteMembershipView[] results = handler.Handle(query).Result.ToArray();

            results.ShouldNotBeNull();
            results.Length.ShouldEqual(1);
            RemoteMembershipView result = results.Single();
            var expectedEntity = data.Single(x => x.UserId == userId);
            result.UserId.ShouldEqual(expectedEntity.UserId);
            result.Provider.ShouldEqual(expectedEntity.LoginProvider);
            result.Key.ShouldEqual(expectedEntity.ProviderKey);
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Once);
        }

        #endregion
    }
}
