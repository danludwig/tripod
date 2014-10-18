using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class EmailAddressByTests
    {
        #region EmailAddressBy Id

        [Fact]
        public void Query_IntCtor_SetsIdProperty()
        {
            var id = new Random().Next(int.MinValue, int.MaxValue);
            var query = new EmailAddressBy(id);
            query.Id.ShouldEqual(id);
            query.Value.ShouldBeNull();
            query.Claim.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNullEmailAddress_ById_WhenNotFound()
        {
            var emailAddressId = new Random().Next(3, int.MaxValue);
            var data = new[] { new EmailAddressWithSpecifiedId(emailAddressId - 1) }.AsQueryable();
            var query = new EmailAddressBy(emailAddressId);
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressByQuery(entities.Object);

            EmailAddress result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handler_ReturnsNullEmailAddress_ById_WhenFound_ButIsVerified_IsNotEqual(bool isVerified)
        {
            var emailAddressId = new Random().Next(1, int.MaxValue);
            var emailAddress = new EmailAddressWithSpecifiedId(emailAddressId)
            {
                IsVerified = !isVerified,
            };
            var data = new[] { emailAddress }.AsQueryable();
            var query = new EmailAddressBy(emailAddressId)
            {
                IsVerified = isVerified
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressByQuery(entities.Object);

            EmailAddress result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(null, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Handler_ReturnsNonNullEmailAddress_ById_WhenFound_AndIsVerifiedMatches(
            bool? queryIsVerified, bool entityIsVerified)
        {
            var emailAddressId = new Random().Next(1, int.MaxValue);
            var emailAddress = new EmailAddressWithSpecifiedId(emailAddressId)
            {
                IsVerified = entityIsVerified,
            };
            var data = new[] { emailAddress }.AsQueryable();
            var query = new EmailAddressBy(emailAddressId)
            {
                IsVerified = queryIsVerified,
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressByQuery(entities.Object);

            EmailAddress result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.ShouldEqual(data.Single());
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        #endregion
        #region EmailAddressBy Value

        [Fact]
        public void Query_StringCtor_SetsValueProperty()
        {
            var value = string.Format("{0}@domain.tld", Guid.NewGuid());
            var query = new EmailAddressBy(value);
            query.Id.ShouldBeNull();
            query.Value.ShouldEqual(value);
            query.Claim.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNullEmailAddress_ByValue_WhenNotFound()
        {
            var emailAddressValue = string.Format("{0}@domain.tld", Guid.NewGuid());
            var emailAddress = new EmailAddress { Value = string.Format("{0}@domain.tld", Guid.NewGuid()) };
            var data = new[] { emailAddress }.AsQueryable();
            var query = new EmailAddressBy(emailAddressValue);
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressByQuery(entities.Object);

            EmailAddress result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handler_ReturnsNullEmailAddress_ByValue_WhenFound_ButIsVerified_IsNotEqual(bool isVerified)
        {
            var emailAddressValue = string.Format("{0}@domain.tld", Guid.NewGuid());
            var emailAddress = new EmailAddress
            {
                Value = emailAddressValue,
                IsVerified = !isVerified,
            };
            var data = new[] { emailAddress }.AsQueryable();
            var query = new EmailAddressBy(emailAddressValue)
            {
                IsVerified = isVerified,
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressByQuery(entities.Object);

            EmailAddress result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(null, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Handler_ReturnsNonNullEmailAddress_ByValue_WhenFound_AndIsVerified_IsEqual(
            bool? isQueryVerified, bool isEntityVerified)
        {
            var emailAddressValue = string.Format("{0}@domain.tld", Guid.NewGuid());
            var emailAddress = new EmailAddress
            {
                Value = emailAddressValue,
                IsVerified = isEntityVerified,
            };
            var data = new[] { emailAddress }.AsQueryable();
            var query = new EmailAddressBy(emailAddressValue)
            {
                IsVerified = isQueryVerified,
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressByQuery(entities.Object);

            EmailAddress result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.ShouldEqual(data.Single());
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        #endregion
        #region EmailAddressBy Claim

        [Fact]
        public void Query_ClaimCtor_SetsClaimProperty()
        {
            var claim = new Claim("claim type", "claim value");
            var query = new EmailAddressBy(claim);
            query.Id.ShouldBeNull();
            query.Value.ShouldBeNull();
            query.Claim.ShouldEqual(claim);
        }

        [Fact]
        public void Handler_ReturnsNullEmailAddress_ByClaim_WhenNotFound()
        {
            var emailAddressValue = string.Format("{0}@domain.tld", Guid.NewGuid());
            var claim = new Claim(ClaimTypes.Email, emailAddressValue);
            var emailAddress = new EmailAddress { Value = string.Format("{0}@domain.tld", Guid.NewGuid()) };
            var data = new[] { emailAddress }.AsQueryable();
            var query = new EmailAddressBy(claim);
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressByQuery(entities.Object);

            EmailAddress result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handler_ReturnsNullEmailAddress_ByClaim_WhenFound_ButIsVerified_IsNotEqual(bool isVerified)
        {
            var emailAddressValue = string.Format("{0}@domain.tld", Guid.NewGuid());
            var claim = new Claim(ClaimTypes.Email, emailAddressValue);
            var emailAddress = new EmailAddress
            {
                Value = emailAddressValue,
                IsVerified = !isVerified,
            };
            var data = new[] { emailAddress }.AsQueryable();
            var query = new EmailAddressBy(claim)
            {
                IsVerified = isVerified,
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressByQuery(entities.Object);

            EmailAddress result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullEmailAddress_ByClaim_WhenClaimType_IsNotEmail()
        {
            var emailAddressValue = string.Format("{0}@domain.tld", Guid.NewGuid());
            var claim = new Claim(ClaimTypes.NameIdentifier, emailAddressValue);
            var emailAddress = new EmailAddress
            {
                Value = emailAddressValue,
            };
            var data = new[] { emailAddress }.AsQueryable();
            var query = new EmailAddressBy(claim);
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressByQuery(entities.Object);

            EmailAddress result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(null, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Handler_ReturnsNonNullEmailAddress_ByClaim_WhenFound_AndIsVerified_IsEqual(
            bool? isQueryVerified, bool isEntityVerified)
        {
            var emailAddressValue = string.Format("{0}@domain.tld", Guid.NewGuid());
            var claim = new Claim(ClaimTypes.Email, emailAddressValue);
            var emailAddress = new EmailAddress
            {
                Value = emailAddressValue,
                IsVerified = isEntityVerified,
            };
            var data = new[] { emailAddress }.AsQueryable();
            var query = new EmailAddressBy(claim)
            {
                IsVerified = isQueryVerified,
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressByQuery(entities.Object);

            EmailAddress result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.ShouldEqual(data.Single());
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        #endregion
    }
}
