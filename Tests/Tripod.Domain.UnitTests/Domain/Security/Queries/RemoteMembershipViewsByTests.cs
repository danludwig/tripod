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
        public void Query_IntCtor_SetsUserIdProperty()
        {
            var userId = FakeData.Id();
            var query = new RemoteMembershipViewsBy(userId);
            query.UserId.ShouldEqual(userId);
        }

        [Fact]
        public void Handler_ReturnsNoRemoteMembershipViews_WhenNotFound_ByUserId()
        {
            var userId = FakeData.Id();
            var otherUserId1 = FakeData.Id(userId);
            var otherUserId2 = FakeData.Id(userId, otherUserId1);
            var remoteMemberships = new[]
            {
                new RemoteMembership { UserId = otherUserId1, },
                new RemoteMembership { UserId = otherUserId2, },
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
        public void Handler_ReturnsRemoteMembershipViews_WhenFound_ByUserId()
        {
            var userId = FakeData.Id();
            var otherUserId1 = FakeData.Id(userId);
            var otherUserId2 = FakeData.Id(userId, otherUserId1);
            var remoteMemberships = new[]
            {
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { UserId = otherUserId1, },
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { UserId = otherUserId2, },
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
