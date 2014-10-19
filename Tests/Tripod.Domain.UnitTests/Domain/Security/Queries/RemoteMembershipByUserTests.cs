using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class RemoteMembershipByUserTests
    {
        #region RemoteMembershipByUser Id

        [Fact]
        public void IntCtor_SetsUserIdProperty_AndUserLoginInfoProperty()
        {
            var userId = new Random().Next(1, int.MaxValue);
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
            var query = new RemoteMembershipByUser(userId, userLoginInfo);
            query.UserId.ShouldEqual(userId);
            query.UserName.ShouldBeNull();
            query.UserLoginInfo.ShouldEqual(userLoginInfo);
        }

        [Fact]
        public void Handle_ReturnsNullRemoteMembership_ByUserId_WhenUserLoginInfo_IsNull()
        {
            var userId = new Random().Next(1, int.MaxValue - 3);
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var remoteMemberships = new[]
            {
                new ProxiedRemoteMembership(loginProvider, providerKey)
                    { UserId = userId + 1, },
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { UserId = userId, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipByUser(userId, null);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipByUserQuery(entities.Object);

            RemoteMembership result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Never);
        }

        [Fact]
        public void Handle_ReturnsNullRemoteMembership_ByUserId_WhenNotFound()
        {
            var userId = new Random().Next(1, int.MaxValue - 3);
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
            var remoteMemberships = new[]
            {
                new ProxiedRemoteMembership(loginProvider, providerKey)
                    { UserId = userId + 1, },
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { UserId = userId, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipByUser(userId, userLoginInfo);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipByUserQuery(entities.Object);

            RemoteMembership result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Once);
        }

        [Fact]
        public void Handle_ReturnsNonNullRemoteMembership_ByUserId_WhenFound()
        {
            var userId = new Random().Next(1, int.MaxValue - 3);
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
            var remoteMemberships = new[]
            {
                new ProxiedRemoteMembership(loginProvider, providerKey)
                    { UserId = userId + 1, },
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { UserId = userId, },
                new ProxiedRemoteMembership(loginProvider, providerKey)
                    { UserId = userId, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipByUser(userId, userLoginInfo);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipByUserQuery(entities.Object);

            RemoteMembership result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            Func<ProxiedRemoteMembership, bool> expectedRemoteMembership =
                x => x.UserId == userId &&
                    x.LoginProvider == loginProvider &&
                    x.ProviderKey == providerKey;
            result.ShouldEqual(remoteMemberships.Single(expectedRemoteMembership));
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Once);
        }

        #endregion
        #region RemoteMembershipByUser Name

        [Fact]
        public void StringCtor_SetsUserNameProperty_AndUserLoginInfoProperty()
        {
            var userName = Guid.NewGuid().ToString();
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
            var query = new RemoteMembershipByUser(userName, userLoginInfo);
            query.UserId.ShouldBeNull();
            query.UserName.ShouldEqual(userName);
            query.UserLoginInfo.ShouldEqual(userLoginInfo);
        }

        [Fact]
        public void Handle_ReturnsNullRemoteMembership_ByUserName_WhenUserLoginInfo_IsNull()
        {
            var userName = Guid.NewGuid().ToString();
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var user = new User { Name = userName };
            var differentUser = new User { Name = Guid.NewGuid().ToString() };
            var remoteMemberships = new[]
            {
                new ProxiedRemoteMembership(loginProvider, providerKey)
                    { User = differentUser, },
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { User = user, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipByUser(userName, null);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipByUserQuery(entities.Object);

            RemoteMembership result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Never);
        }

        [Fact]
        public void Handle_ReturnsNullRemoteMembership_ByUserName_WhenNotFound()
        {
            var userName = Guid.NewGuid().ToString();
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
            var user = new User { Name = userName };
            var differentUser = new User { Name = Guid.NewGuid().ToString() };
            var remoteMemberships = new[]
            {
                new ProxiedRemoteMembership(loginProvider, providerKey)
                    { User = differentUser, },
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { User = user, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipByUser(userName, userLoginInfo);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipByUserQuery(entities.Object);

            RemoteMembership result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Once);
        }

        [Fact]
        public void Handle_ReturnsNonNullRemoteMembership_ByUserName_WhenFound()
        {
            var userName = Guid.NewGuid().ToString();
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
            var user = new User { Name = userName };
            var differentUser = new User { Name = Guid.NewGuid().ToString() };
            var remoteMemberships = new[]
            {
                new ProxiedRemoteMembership(loginProvider, providerKey)
                    { User = differentUser, },
                new ProxiedRemoteMembership(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                    { User = user, },
                new ProxiedRemoteMembership(loginProvider, providerKey)
                    { User = user, },
            };
            var data = remoteMemberships.AsQueryable();
            var query = new RemoteMembershipByUser(userName, userLoginInfo);
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<RemoteMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<RemoteMembership>()).Returns(entitySet);
            var handler = new HandleRemoteMembershipByUserQuery(entities.Object);

            RemoteMembership result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            Func<ProxiedRemoteMembership, bool> expectedRemoteMembership =
                x => x.User.Name == userName &&
                    x.LoginProvider == loginProvider &&
                    x.ProviderKey == providerKey;
            result.ShouldEqual(remoteMemberships.Single(expectedRemoteMembership));
            entities.Verify(x => x.Query<RemoteMembership>(), Times.Once);
        }

        #endregion
    }
}
