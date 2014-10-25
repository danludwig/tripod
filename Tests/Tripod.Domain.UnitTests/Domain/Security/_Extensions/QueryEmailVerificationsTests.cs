using System;
using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class QueryEmailVerificationsTests
    {
        #region ByTicket

        [Fact]
        public void ByTicket_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
            };
            data.AsQueryable().ByTicket(FakeData.String()).ShouldBeNull();
        }

        [Fact]
        public void ByTicket_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
            };
            data.AsQueryable().ByTicket(data[0].Ticket, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsQueryable().ByTicket(FakeData.String(), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByTicket_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
            };
            data.AsEnumerable().ByTicket(FakeData.String()).ShouldBeNull();
        }

        [Fact]
        public void ByTicket_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
            };
            data.AsEnumerable().ByTicket(data[0].Ticket, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsEnumerable().ByTicket(FakeData.String(), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByTicketAsync_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
            };
            var dbSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByTicketAsync(FakeData.String())
                .Result.ShouldBeNull();
        }

        [Fact]
        public void ByTicketAsync_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
            };
            var dbSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByTicketAsync(data[0].Ticket, false).Result.ShouldNotBeNull();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsQueryable().ByTicketAsync(FakeData.String(), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByTicketAsync_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
            };
            var dbSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByTicketAsync(FakeData.String())
                .Result.ShouldBeNull();
        }

        [Fact]
        public void ByTicketAsync_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = FakeData.String(), },
            };
            var dbSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByTicketAsync(data[0].Ticket, false).Result.ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByTicketAsync(FakeData.String(), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        #endregion
        #region ByEmailAddressId

        [Fact]
        public void ByEmailAddressId_Queryable_FiltersByEmailAddressId()
        {
            var emailAddressId = FakeData.Id();
            var data = new[]
            {
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = FakeData.Id(canNotBe: emailAddressId),
                },
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = emailAddressId,
                },
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = emailAddressId,
                },
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = FakeData.Id(canNotBe: emailAddressId),
                },
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = FakeData.Id(canNotBe: emailAddressId),
                },
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = emailAddressId,
                },
            };

            var results = data.AsQueryable().ByEmailAddressId(emailAddressId).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        [Fact]
        public void ByEmailAddressId_Enumerable_FiltersByEmailAddressId()
        {
            var emailAddressId = FakeData.Id();
            var data = new[]
            {
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = FakeData.Id(canNotBe: emailAddressId),
                },
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = emailAddressId,
                },
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = emailAddressId,
                },
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = FakeData.Id(canNotBe: emailAddressId),
                },
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = FakeData.Id(canNotBe: emailAddressId),
                },
                new EmailVerification
                {
                    Ticket = FakeData.String(),
                    EmailAddressId = emailAddressId,
                },
            };

            var results = data.AsEnumerable().ByEmailAddressId(emailAddressId).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        #endregion
    }
}
