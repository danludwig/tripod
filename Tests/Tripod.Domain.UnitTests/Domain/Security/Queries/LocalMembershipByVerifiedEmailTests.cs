using System;
using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class LocalMembershipByVerifiedEmailTests
    {
        [Fact]
        public void Query_StringCtor_SetsEmailAddressProperty()
        {
            var emailAddress = FakeData.Email();
            var query = new LocalMembershipByVerifiedEmail(emailAddress);
            query.EmailAddress.ShouldEqual(emailAddress);
        }

        [Fact]
        public void Handler_ReturnsNullLocalMembership_WhenFoundButNotVerified_ByVerifiedEmail()
        {
            var emailAddress = FakeData.Email();
            var user = new User();
            user.EmailAddresses.Add(new EmailAddress
            {
                IsVerified = true,
                Value = FakeData.Email() ,
            });
            var data = new[]
            {
                new LocalMembership { User = user, },
            }.AsQueryable();
            var query = new LocalMembershipByVerifiedEmail(emailAddress);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleLocalMembershipByVerifiedEmailQuery(entities.Object);

            LocalMembership result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullLocalMembership_WhenNotFound_ByVerifiedEmail()
        {
            var emailAddress = FakeData.Email();
            var user = new User();
            user.EmailAddresses.Add(new EmailAddress
            {
                IsVerified = false,
                Value = emailAddress,
            });
            var data = new[]
            {
                new LocalMembership { User = user, },
            }.AsQueryable();
            var query = new LocalMembershipByVerifiedEmail(emailAddress);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleLocalMembershipByVerifiedEmailQuery(entities.Object);

            LocalMembership result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNonNullLocalMembership_WhenFound_ByUserName()
        {
            var emailAddress = FakeData.Email();
            var user = new User();
            user.EmailAddresses.Add(new EmailAddress
            {
                IsVerified = true,
                Value = emailAddress,
            });
            var data = new[]
            {
                new LocalMembership
                {
                    User = user,
                    PasswordHash = "password hash",
                },
            }.AsQueryable();
            var query = new LocalMembershipByVerifiedEmail(emailAddress);
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<LocalMembership>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<LocalMembership>()).Returns(entitySet);
            var handler = new HandleLocalMembershipByVerifiedEmailQuery(entities.Object);

            LocalMembership result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.User.ShouldEqual(user);
            result.PasswordHash.ShouldEqual("password hash");
            entities.Verify(x => x.Query<LocalMembership>(), Times.Once);
        }
    }
}
