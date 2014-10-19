using System;
using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class RemoteMembershipsByUserTests
    {
        #region RemoteMembershipsByUser Id

        [Fact]
        public void Query_IntCtor_SetsUserIdProperty()
        {
            var userId = new Random().Next(1, int.MaxValue);
            var query = new RemoteMembershipsByUser(userId);
            query.UserId.ShouldEqual(userId);
            query.UserName.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNoRemoteMemberships_WhenNotFound_ByUserId()
        {
            var userId = new Random().Next(1, int.MaxValue - 3);
            var remoteMemberships = new[]
            {
                new RemoteMembership { UserId = userId + 1, },
                new RemoteMembership { UserId = userId - 1, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipsByUser(userId);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipsByUserQuery(entities.Object);

            RemoteMembership[] result = handler.Handle(query).Result.ToArray();

            result.ShouldNotBeNull();
            result.Length.ShouldEqual(0);
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsRemoteMemberships_WhenFound_ByUserId()
        {
            var userId = new Random().Next(1, int.MaxValue - 3);
            var remoteMemberships = new[]
            {
                new RemoteMembership { UserId = userId + 1, },
                new RemoteMembership { UserId = userId - 1, },
                new RemoteMembership { UserId = userId, },
                new RemoteMembership { UserId = userId, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipsByUser(userId);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipsByUserQuery(entities.Object);

            RemoteMembership[] result = handler.Handle(query).Result.ToArray();

            result.ShouldNotBeNull();
            result.Length.ShouldEqual(2);
            var expectedResults = data.Where(x => x.UserId == userId);
            result.ShouldContain(expectedResults.First());
            result.ShouldContain(expectedResults.Skip(1).First());
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Once);
        }

        #endregion
        #region RemoteMembershipsByUser Name

        [Fact]
        public void Query_StringCtor_SetsUserNameProperty()
        {
            var userName = Guid.NewGuid().ToString();
            var query = new RemoteMembershipsByUser(userName);
            query.UserId.ShouldBeNull();
            query.UserName.ShouldEqual(userName);
        }

        [Fact]
        public void Handler_ReturnsNoRemoteMemberships_WhenNotFound_ByUserName()
        {
            var userName = Guid.NewGuid().ToString();
            var differentUser1 = new User { Name = Guid.NewGuid().ToString(), };
            var differentUser2 = new User { Name = Guid.NewGuid().ToString(), };
            var remoteMemberships = new[]
            {
                new RemoteMembership { User = differentUser1, },
                new RemoteMembership { User = differentUser2, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipsByUser(userName);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipsByUserQuery(entities.Object);

            RemoteMembership[] result = handler.Handle(query).Result.ToArray();

            result.ShouldNotBeNull();
            result.Length.ShouldEqual(0);
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsRemoteMemberships_WhenFound_ByUserName()
        {
            var userName = Guid.NewGuid().ToString();
            var user = new User { Name = userName, };
            var differentUser1 = new User { Name = Guid.NewGuid().ToString(), };
            var differentUser2 = new User { Name = Guid.NewGuid().ToString(), };
            var remoteMemberships = new[]
            {
                new RemoteMembership { User = differentUser1, },
                new RemoteMembership { User = differentUser2, },
                new RemoteMembership { User = user, },
                new RemoteMembership { User = user, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipsByUser(userName);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipsByUserQuery(entities.Object);

            RemoteMembership[] result = handler.Handle(query).Result.ToArray();

            result.ShouldNotBeNull();
            result.Length.ShouldEqual(2);
            var expectedResults = data.Where(x => x.User.Name == userName);
            result.ShouldContain(expectedResults.First());
            result.ShouldContain(expectedResults.Skip(1).First());
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Once);
        }

        #endregion
    }
}
