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
    public class UserViewByTests
    {
        #region UserViewBy Id

        [Fact]
        public void Query_IntCtor_SetsIdProperty()
        {
            var id = FakeData.Id();
            var query = new UserViewBy(id);
            query.Id.ShouldEqual(id);
            query.Name.ShouldBeNull();
            query.Principal.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNullUserView_WhenNotFound_ById()
        {
            var userId = FakeData.Id();
            var otherUserId = FakeData.Id(canNotBe: userId);
            var user = new ProxiedUser(otherUserId) { Name = FakeData.String(), };
            var primaryEmail = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsPrimary = true,
                IsVerified = true,
            };
            var secondaryEmail1 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsVerified = true,
            };
            var secondaryEmail2 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
            };
            user.EmailAddresses.Add(secondaryEmail1);
            user.EmailAddresses.Add(secondaryEmail2);
            user.EmailAddresses.Add(primaryEmail);
            var data = new[] { user }.AsQueryable();
            var query = new UserViewBy(userId);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            IQueryable<User> toBeReturned = entitySet;
            entities.Setup(x => x.Query<User>()).Returns(toBeReturned);
            var handler = new HandleUserViewByQuery(entities.Object);

            UserView result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNonNullUserView_WhenFound_ById()
        {
            var userId = FakeData.Id();
            var user = new ProxiedUser(userId) { Name = FakeData.String(), };
            var primaryEmail = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsPrimary = true,
                IsVerified = true,
            };
            var secondaryEmail1 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsVerified = true,
            };
            var secondaryEmail2 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
            };
            user.EmailAddresses.Add(secondaryEmail1);
            user.EmailAddresses.Add(secondaryEmail2);
            user.EmailAddresses.Add(primaryEmail);
            var data = new[] { user }.AsQueryable();
            var query = new UserViewBy(userId);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            IQueryable<User> toBeReturned = entitySet;
            entities.Setup(x => x.Query<User>()).Returns(toBeReturned);
            var handler = new HandleUserViewByQuery(entities.Object);

            UserView result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.UserId.ShouldEqual(user.Id);
            result.UserName.ShouldEqual(user.Name);
            result.PrimaryEmailAddress.ShouldEqual(primaryEmail.Value);
            result.PrimaryEmailHash.ShouldEqual(primaryEmail.HashedValue);
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        #endregion
        #region UserViewBy Name

        [Fact]
        public void Query_StringCtor_SetsNameProperty()
        {
            var name = FakeData.String();
            var query = new UserViewBy(name);
            query.Id.ShouldBeNull();
            query.Name.ShouldEqual(name);
            query.Principal.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNullUserView_WhenNotFound_ByName()
        {
            var userId = FakeData.Id();
            var userName = FakeData.String();
            var user = new ProxiedUser(userId) { Name = FakeData.String(), };
            var primaryEmail = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsPrimary = true,
                IsVerified = true,
            };
            var secondaryEmail1 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsVerified = true,
            };
            var secondaryEmail2 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
            };
            user.EmailAddresses.Add(secondaryEmail1);
            user.EmailAddresses.Add(secondaryEmail2);
            user.EmailAddresses.Add(primaryEmail);
            var data = new[] { user }.AsQueryable();
            var query = new UserViewBy(userName);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            IQueryable<User> toBeReturned = entitySet;
            entities.Setup(x => x.Query<User>()).Returns(toBeReturned);
            var handler = new HandleUserViewByQuery(entities.Object);

            UserView result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNonNullUserView_WhenFound_ByName()
        {
            var userId = FakeData.Id();
            var userName = FakeData.String();
            var user = new ProxiedUser(userId) { Name = userName, };
            var primaryEmail = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsPrimary = true,
                IsVerified = true,
            };
            var secondaryEmail1 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsVerified = true,
            };
            var secondaryEmail2 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
            };
            user.EmailAddresses.Add(secondaryEmail1);
            user.EmailAddresses.Add(secondaryEmail2);
            user.EmailAddresses.Add(primaryEmail);
            var data = new[] { user }.AsQueryable();
            var query = new UserViewBy(userName);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            IQueryable<User> toBeReturned = entitySet;
            entities.Setup(x => x.Query<User>()).Returns(toBeReturned);
            var handler = new HandleUserViewByQuery(entities.Object);

            UserView result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.UserId.ShouldEqual(user.Id);
            result.UserName.ShouldEqual(user.Name);
            result.PrimaryEmailAddress.ShouldEqual(primaryEmail.Value);
            result.PrimaryEmailHash.ShouldEqual(primaryEmail.HashedValue);
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        #endregion
        #region UserViewBy Principal

        [Fact]
        public void Query_PrincipalCtor_SetsPrincipalProperty()
        {
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var query = new UserViewBy(principal.Object);
            query.Id.ShouldBeNull();
            query.Name.ShouldBeNull();
            query.Principal.ShouldEqual(principal.Object);
        }

        [Fact]
        public void Handler_ReturnsNullUserView_WhenPrincipalIsNull()
        {
            var userId = FakeData.Id();
            var user = new ProxiedUser(userId) { Name = FakeData.String(), };
            var data = new[] { user }.AsQueryable();
            var query = new UserViewBy(null as IPrincipal);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            IQueryable<User> toBeReturned = entitySet;
            entities.Setup(x => x.Query<User>()).Returns(toBeReturned);
            var handler = new HandleUserViewByQuery(entities.Object);

            UserView result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullUserView_WhenPrincipalIdentity_IsNotAuthenticated()
        {
            var userId = FakeData.Id();
            var user = new ProxiedUser(userId) { Name = FakeData.String(), };
            var data = new[] { user }.AsQueryable();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)), 
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new GenericPrincipal(identity, null);
            var query = new UserViewBy(principal);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            IQueryable<User> toBeReturned = entitySet;
            entities.Setup(x => x.Query<User>()).Returns(toBeReturned);
            var handler = new HandleUserViewByQuery(entities.Object);

            UserView result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullUserView_WhenPrincipalIdentity_IsNotClaimsIdentity()
        {
            var userId = FakeData.Id();
            var user = new ProxiedUser(userId) { Name = FakeData.String(), };
            var data = new[] { user }.AsQueryable();
            var identity = new GenericIdentity(user.Name, "authentication type");
            var principal = new GenericPrincipal(identity, null);
            var query = new UserViewBy(principal);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            IQueryable<User> toBeReturned = entitySet;
            entities.Setup(x => x.Query<User>()).Returns(toBeReturned);
            var handler = new HandleUserViewByQuery(entities.Object);

            UserView result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullUserView_WhenPrincipalIdentity_HasNoNameIdentifierClaim()
        {
            var userId = FakeData.Id();
            var user = new ProxiedUser(userId) { Name = FakeData.String(), };
            var data = new[] { user }.AsQueryable();
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userId.ToString(CultureInfo.InvariantCulture)), 
            };
            var identity = new ClaimsIdentity(claims, "authentication type");
            var principal = new GenericPrincipal(identity, null);
            var query = new UserViewBy(principal);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            IQueryable<User> toBeReturned = entitySet;
            entities.Setup(x => x.Query<User>()).Returns(toBeReturned);
            var handler = new HandleUserViewByQuery(entities.Object);

            UserView result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullUserView_WhenNotFound_ByPrincipal()
        {
            var userId = FakeData.Id();
            var otherUserId = FakeData.Id(canNotBe: userId);
            var user = new ProxiedUser(otherUserId) { Name = FakeData.String(), };
            var primaryEmail = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsPrimary = true,
                IsVerified = true,
            };
            var secondaryEmail1 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsVerified = true,
            };
            var secondaryEmail2 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
            };
            user.EmailAddresses.Add(secondaryEmail1);
            user.EmailAddresses.Add(secondaryEmail2);
            user.EmailAddresses.Add(primaryEmail);
            var data = new[] { user }.AsQueryable();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)), 
            };
            var identity = new ClaimsIdentity(claims, "authenticationType");
            var principal = new GenericPrincipal(identity, null);
            var query = new UserViewBy(principal);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            IQueryable<User> toBeReturned = entitySet;
            entities.Setup(x => x.Query<User>()).Returns(toBeReturned);
            var handler = new HandleUserViewByQuery(entities.Object);

            UserView result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNonNullUserView_WhenFound_ByPrincipal()
        {
            var userId = FakeData.Id();
            var user = new ProxiedUser(userId) { Name = FakeData.String(), };
            var primaryEmail = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsPrimary = true,
                IsVerified = true,
            };
            var secondaryEmail1 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
                IsVerified = true,
            };
            var secondaryEmail2 = new EmailAddress
            {
                Value = FakeData.Email(),
                HashedValue = FakeData.String(),
            };
            user.EmailAddresses.Add(secondaryEmail1);
            user.EmailAddresses.Add(secondaryEmail2);
            user.EmailAddresses.Add(primaryEmail);
            var data = new[] { user }.AsQueryable();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)), 
            };
            var identity = new ClaimsIdentity(claims, "authentication type");
            var principal = new GenericPrincipal(identity, null);
            var query = new UserViewBy(principal);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            IQueryable<User> toBeReturned = entitySet;
            entities.Setup(x => x.Query<User>()).Returns(toBeReturned);
            var handler = new HandleUserViewByQuery(entities.Object);

            UserView result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.UserId.ShouldEqual(user.Id);
            result.UserName.ShouldEqual(user.Name);
            result.PrimaryEmailAddress.ShouldEqual(primaryEmail.Value);
            result.PrimaryEmailHash.ShouldEqual(primaryEmail.HashedValue);
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        #endregion
    }
}
