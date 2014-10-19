using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class UserHasLocalMembershipTests
    {
        #region UserHasLocalMembership ById

        [Fact]
        public void Query_IntCtor_SetsUserIdProperty()
        {
            var userId = new Random().Next(int.MinValue, int.MaxValue);
            var query = new UserHasLocalMembership(userId);
            query.UserId.ShouldEqual(userId);
            query.UserName.ShouldBeNull();
            query.Principal.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsFalse_WhenNotFound_ByUserId()
        {
            var userId = new Random().Next(3, int.MaxValue - 3);
            var user = new ProxiedUser(userId);
            var differentUser1 = new ProxiedUser(userId + 1);
            var differentUser2 = new ProxiedUser(userId - 1);
            var localMemberships = new[]
            {
                new LocalMembership { User = differentUser1, },
                new LocalMembership { User = differentUser2, },
            };
            var data = localMemberships.AsQueryable();
            var query = new UserHasLocalMembership(user.Id);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleUserHasLocalMembershipQuery(entities.Object);

            bool result = handler.Handle(query).Result;

            result.ShouldBeFalse();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsTrue_WhenFound_ByUserId()
        {
            var userId = new Random().Next(3, int.MaxValue - 3);
            var user = new ProxiedUser(userId);
            var differentUser1 = new ProxiedUser(userId + 1);
            var differentUser2 = new ProxiedUser(userId - 1);
            var localMemberships = new[]
            {
                new LocalMembership { User = differentUser1, },
                new LocalMembership { User = user, },
                new LocalMembership { User = differentUser2, },
            };
            var data = localMemberships.AsQueryable();
            var query = new UserHasLocalMembership(user.Id);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleUserHasLocalMembershipQuery(entities.Object);

            bool result = handler.Handle(query).Result;

            result.ShouldBeTrue();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        #endregion
        #region UserHasLocalMembership ByUserName

        [Fact]
        public void Query_StringCtor_SetsUserNameProperty()
        {
            var userName = Guid.NewGuid().ToString();
            var query = new UserHasLocalMembership(userName);
            query.UserId.ShouldBeNull();
            query.UserName.ShouldEqual(userName);
            query.Principal.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsFalse_WhenNotFound_ByUserName()
        {
            var userName = Guid.NewGuid().ToString();
            var user = new User { Name = userName, };
            var differentUser1 = new User { Name = Guid.NewGuid().ToString(), };
            var differentUser2 = new User { Name = Guid.NewGuid().ToString(), };
            var localMemberships = new[]
            {
                new LocalMembership { User = differentUser1, },
                new LocalMembership { User = differentUser2, },
            };
            var data = localMemberships.AsQueryable();
            var query = new UserHasLocalMembership(user.Name);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleUserHasLocalMembershipQuery(entities.Object);

            bool result = handler.Handle(query).Result;

            result.ShouldBeFalse();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsTrue_WhenFound_ByUserName()
        {
            var userName = Guid.NewGuid().ToString();
            var user = new User { Name = userName, };
            var differentUser1 = new User { Name = Guid.NewGuid().ToString(), };
            var differentUser2 = new User { Name = Guid.NewGuid().ToString(), };
            var localMemberships = new[]
            {
                new LocalMembership { User = differentUser1, },
                new LocalMembership { User = user, },
                new LocalMembership { User = differentUser2, },
            };
            var data = localMemberships.AsQueryable();
            var query = new UserHasLocalMembership(user.Name);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleUserHasLocalMembershipQuery(entities.Object);

            bool result = handler.Handle(query).Result;

            result.ShouldBeTrue();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        #endregion
        #region UserHasLocalMembership ByPrincipal

        [Fact]
        public void Query_PrincipalCtor_SetsPrincipalProperty()
        {
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var query = new UserHasLocalMembership(principal.Object);
            query.UserId.ShouldBeNull();
            query.UserName.ShouldBeNull();
            query.Principal.ShouldEqual(principal.Object);
        }

        [Fact]
        public void Handler_ReturnsFalse_WhenPrincipal_IsNull()
        {
            var userId = new Random().Next(1, int.MaxValue);
            var user = new ProxiedUser(userId) { Name = Guid.NewGuid().ToString(), };
            var differentUser1 = new ProxiedUser(userId + 1) { Name = Guid.NewGuid().ToString(), };
            var differentUser2 = new ProxiedUser(userId - 1) { Name = Guid.NewGuid().ToString(), };
            var localMemberships = new[]
            {
                new LocalMembership { User = differentUser1, },
                new LocalMembership { User = user, },
                new LocalMembership { User = differentUser2, },
            };
            var data = localMemberships.AsQueryable();
            var query = new UserHasLocalMembership(null as IPrincipal);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleUserHasLocalMembershipQuery(entities.Object);

            bool result = handler.Handle(query).Result;

            result.ShouldBeFalse();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsFalse_WhenPrincipalIdentity_HasNoNameIdentifierClaim()
        {
            var userId = new Random().Next(1, int.MaxValue);
            var user = new ProxiedUser(userId) { Name = Guid.NewGuid().ToString(), };
            var differentUser1 = new ProxiedUser(userId + 1) { Name = Guid.NewGuid().ToString(), };
            var differentUser2 = new ProxiedUser(userId - 1) { Name = Guid.NewGuid().ToString(), };
            var localMemberships = new[]
            {
                new LocalMembership { User = differentUser1, },
                new LocalMembership { User = user, },
                new LocalMembership { User = differentUser2, },
            };
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString(CultureInfo.InvariantCulture)),
            });
            var principal = new GenericPrincipal(identity, null);
            var data = localMemberships.AsQueryable();
            var query = new UserHasLocalMembership(principal);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleUserHasLocalMembershipQuery(entities.Object);

            bool result = handler.Handle(query).Result;

            result.ShouldBeFalse();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsFalse_WhenPrincipalIdentity_IsNotClaimsIdentity()
        {
            var userId = new Random().Next(1, int.MaxValue);
            var user = new ProxiedUser(userId) { Name = Guid.NewGuid().ToString(), };
            var differentUser1 = new ProxiedUser(userId + 1) { Name = Guid.NewGuid().ToString(), };
            var differentUser2 = new ProxiedUser(userId - 1) { Name = Guid.NewGuid().ToString(), };
            var localMemberships = new[]
            {
                new LocalMembership { User = differentUser1, },
                new LocalMembership { User = user, },
                new LocalMembership { User = differentUser2, },
            };
            var identity = new GenericIdentity(user.Name);
            var principal = new GenericPrincipal(identity, null);
            var data = localMemberships.AsQueryable();
            var query = new UserHasLocalMembership(principal);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleUserHasLocalMembershipQuery(entities.Object);

            bool result = handler.Handle(query).Result;

            result.ShouldBeFalse();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsFalse_WhenNotFound_ByPrincipal()
        {
            var userId = new Random().Next(1, int.MaxValue);
            var user = new ProxiedUser(userId);
            var differentUser1 = new ProxiedUser(userId + 1);
            var differentUser2 = new ProxiedUser(userId - 1);
            var localMemberships = new[]
            {
                new LocalMembership { User = differentUser1, },
                new LocalMembership { User = differentUser2, },
            };
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
            });
            var principal = new GenericPrincipal(identity, null);
            var data = localMemberships.AsQueryable();
            var query = new UserHasLocalMembership(principal);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleUserHasLocalMembershipQuery(entities.Object);

            bool result = handler.Handle(query).Result;

            result.ShouldBeFalse();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsTrue_WhenFound_ByPrincipal()
        {
            var userId = new Random().Next(1, int.MaxValue);
            var user = new ProxiedUser(userId) { Name = Guid.NewGuid().ToString(), };
            var differentUser1 = new ProxiedUser(userId + 1) { Name = Guid.NewGuid().ToString(), };
            var differentUser2 = new ProxiedUser(userId - 1) { Name = Guid.NewGuid().ToString(), };
            var localMemberships = new[]
            {
                new LocalMembership { User = differentUser1, },
                new LocalMembership { User = user, },
                new LocalMembership { User = differentUser2, },
            };
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
            });
            var principal = new GenericPrincipal(identity, null);
            var data = localMemberships.AsQueryable();
            var query = new UserHasLocalMembership(principal);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleUserHasLocalMembershipQuery(entities.Object);

            bool result = handler.Handle(query).Result;

            result.ShouldBeTrue();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        #endregion
    }
}
