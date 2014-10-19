using System;
using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class EmailVerificationByTests
    {
        #region EmailVerificationBy Ticket

        [Fact]
        public void Query_StringCtor_SetsTicketProperty()
        {
            var ticket = Guid.NewGuid().ToString();
            var query = new EmailVerificationBy(ticket);
            query.Ticket.ShouldEqual(ticket);
        }

        [Fact]
        public void Handler_ReturnsNullEmailVerification_WhenNotFound_ByTicket()
        {
            var ticket = Guid.NewGuid().ToString();
            var emailVerification = new EmailVerification
            {
                Ticket = Guid.NewGuid().ToString(),
            };
            var data = new[] { emailVerification }.AsQueryable();
            var query = new EmailVerificationBy(ticket);
            var dbSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailVerification>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailVerification>()).Returns(entitySet);
            var handler = new HandleEmailVerificationByQuery(entities.Object);

            EmailVerification result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<EmailVerification>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNonNullEmailVerification_WhenFound_ByTicket()
        {
            var ticket = Guid.NewGuid().ToString();
            var emailVerification = new EmailVerification
            {
                Ticket = ticket,
            };
            var data = new[] { emailVerification }.AsQueryable();
            var query = new EmailVerificationBy(ticket);
            var dbSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailVerification>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailVerification>()).Returns(entitySet);
            var handler = new HandleEmailVerificationByQuery(entities.Object);

            EmailVerification result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.ShouldEqual(data.Single());
            entities.Verify(x => x.Query<EmailVerification>(), Times.Once);
        }

        #endregion
    }
}
