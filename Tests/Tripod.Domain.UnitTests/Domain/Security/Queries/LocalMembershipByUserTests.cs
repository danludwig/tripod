using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class LocalMembershipByUserTests
    {
        #region LocalMembershipByUser UserId

        [Fact]
        public void Query_IntCtor_SetsUserIdProperty()
        {
            var userId = FakeData.Id();
            var query = new LocalMembershipByUser(userId);
            query.UserId.ShouldEqual(userId);
            query.UserName.ShouldBeNull();
            query.UserLoginInfo.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNullLocalMembership_WhenNotFound_ByUserId()
        {
            var userId = FakeData.Id();
            var otherUserId = FakeData.Id(canNotBe: userId);
            var user = new ProxiedUser(otherUserId);
            var data = new[]
            {
                new LocalMembership { User = user, },
            }.AsQueryable();
            var query = new LocalMembershipByUser(userId);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleLocalMembershipByUserQuery(entities.Object);

            LocalMembership result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNonNullLocalMembership_WhenFound_ByUserId()
        {
            var userId = FakeData.Id();
            var user = new ProxiedUser(userId);
            var data = new[]
            {
                new LocalMembership
                {
                    User = user,
                    PasswordHash = "password hash",
                },
            }.AsQueryable();
            var query = new LocalMembershipByUser(userId);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleLocalMembershipByUserQuery(entities.Object);

            LocalMembership result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.User.ShouldEqual(user);
            result.PasswordHash.ShouldEqual("password hash");
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        #endregion
        #region LocalMembershipByUser UserName

        [Fact]
        public void Query_StringCtor_SetsUserNameProperty()
        {
            var userName = Guid.NewGuid().ToString();
            var query = new LocalMembershipByUser(userName);
            query.UserId.ShouldBeNull();
            query.UserName.ShouldEqual(userName);
            query.UserLoginInfo.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNullLocalMembership_WhenNotFound_ByUserName()
        {
            var userName = Guid.NewGuid().ToString();
            var user = new User { Name = Guid.NewGuid().ToString() };
            var data = new[]
            {
                new LocalMembership { User = user, },
            }.AsQueryable();
            var query = new LocalMembershipByUser(userName);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleLocalMembershipByUserQuery(entities.Object);

            LocalMembership result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNonNullLocalMembership_WhenFound_ByUserName()
        {
            var userName = Guid.NewGuid().ToString();
            var user = new User { Name = userName };
            var data = new[]
            {
                new LocalMembership
                {
                    User = user,
                    PasswordHash = "password hash",
                },
            }.AsQueryable();
            var query = new LocalMembershipByUser(userName);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleLocalMembershipByUserQuery(entities.Object);

            LocalMembership result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.User.ShouldEqual(user);
            result.PasswordHash.ShouldEqual("password hash");
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        #endregion
        #region LocalMembershipByUser UserName

        [Fact]
        public void Query_UserLoginInfoCtor_SetsUserLoginInfoProperty()
        {
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
            var query = new LocalMembershipByUser(userLoginInfo);
            query.UserId.ShouldBeNull();
            query.UserName.ShouldBeNull();
            query.UserLoginInfo.ShouldEqual(userLoginInfo);
            query.UserLoginInfo.LoginProvider.ShouldEqual(loginProvider);
            query.UserLoginInfo.ProviderKey.ShouldEqual(providerKey);
        }

        [Fact]
        public void Handler_ReturnsNullLocalMembership_WhenNotFound_ByUserLoginInfo()
        {
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
            var user = new User();
            user.RemoteMemberships.Add(new ProxiedRemoteMembership(
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            var data = new[]
            {
                new LocalMembership { User = user, },
            }.AsQueryable();
            var query = new LocalMembershipByUser(userLoginInfo);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleLocalMembershipByUserQuery(entities.Object);

            LocalMembership result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNonNullLocalMembership_WhenFound_ByUserLoginInfo()
        {
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
            var user = new User();
            user.RemoteMemberships.Add(new ProxiedRemoteMembership(
                loginProvider, providerKey));
            var data = new[]
            {
                new LocalMembership
                {
                    User = user,
                    PasswordHash = "password hash",
                },
            }.AsQueryable();
            var query = new LocalMembershipByUser(userLoginInfo);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleLocalMembershipByUserQuery(entities.Object);

            LocalMembership result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.User.ShouldEqual(user);
            result.PasswordHash.ShouldEqual("password hash");
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        #endregion
    }
}
