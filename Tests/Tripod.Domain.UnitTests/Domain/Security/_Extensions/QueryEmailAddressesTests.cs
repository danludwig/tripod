using System;
using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class QueryEmailAddressesTests
    {
        #region ByValue

        [Fact]
        public void ByValue_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
            };
            data.AsQueryable().ByValue(FakeData.Email()).ShouldBeNull();
        }

        [Fact]
        public void ByValue_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
            };
            data.AsQueryable().ByValue(data[0].Value, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsQueryable().ByValue(FakeData.Email(), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByValue_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
            };
            data.AsEnumerable().ByValue(FakeData.Email()).ShouldBeNull();
        }

        [Fact]
        public void ByValue_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
            };
            data.AsEnumerable().ByValue(data[0].Value, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsEnumerable().ByValue(FakeData.Email(), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByValueAsync_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByValueAsync(FakeData.Email())
                .Result.ShouldBeNull();
        }

        [Fact]
        public void ByValueAsync_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByValueAsync(data[0].Value, false).Result.ShouldNotBeNull();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsQueryable().ByValueAsync(FakeData.Email(), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element",StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByValueAsync_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByValueAsync(FakeData.Email())
                .Result.ShouldBeNull();
        }

        [Fact]
        public void ByValueAsync_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
                new EmailAddress { Value = FakeData.Email(), },
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByValueAsync(data[0].Value, false).Result.ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByValueAsync(FakeData.Email(), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        #endregion
        #region ByUserId

        [Fact]
        public void ByUserId_Queryable_FiltersByUserId()
        {
            var userId = new Random().Next(1, int.MaxValue - 100000);
            var data = new[]
            {
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = new Random().Next(int.MaxValue - 10000, int.MaxValue),
                },
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = userId,
                },
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = userId,
                },
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = new Random().Next(int.MaxValue - 10000, int.MaxValue),
                },
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = new Random().Next(int.MaxValue - 10000, int.MaxValue),
                },
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = userId,
                },
            };

            var results = data.AsQueryable().ByUserId(userId).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        [Fact]
        public void ByUserId_Enumerable_FiltersByUserId()
        {
            var userId = new Random().Next(1, int.MaxValue - 100000);
            var data = new[]
            {
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = new Random().Next(int.MaxValue - 10000, int.MaxValue),
                },
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = userId,
                },
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = userId,
                },
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = new Random().Next(int.MaxValue - 10000, int.MaxValue),
                },
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = new Random().Next(int.MaxValue - 10000, int.MaxValue),
                },
                new EmailAddress
                {
                    Value = FakeData.Email(),
                    UserId = userId,
                },
            };

            var results = data.AsEnumerable().ByUserId(userId).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        #endregion
    }
}
