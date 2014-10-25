using System;
using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class QueryLocalMembershipsTests
    {
        #region ByUserId

        [Fact]
        public void ByUserId_Queryable_CanAllowNull()
        {
            var userId = new Random().Next(1, 99);
            var data = new[]
            {
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
                new LocalMembership { User = new ProxiedUser(userId), },
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
            };
            data.AsQueryable().ByUserId(new Random().Next(1000, int.MaxValue)).ShouldBeNull();
        }

        [Fact]
        public void ByUserId_Queryable_CanDisallowNull()
        {
            var userId = new Random().Next(1, 99);
            var data = new[]
            {
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
                new LocalMembership { User = new ProxiedUser(userId), },
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
            };
            data.AsQueryable().ByUserId(data[1].User.Id, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsQueryable().ByUserId(new Random().Next(1000, int.MaxValue), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserId_Enumerable_CanAllowNull()
        {
            var userId = new Random().Next(1, 99);
            var data = new[]
            {
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
                new LocalMembership { User = new ProxiedUser(userId), },
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
            };
            data.AsEnumerable().ByUserId(new Random().Next(1000, int.MaxValue)).ShouldBeNull();
        }

        [Fact]
        public void ByUserId_Enumerable_CanDisallowNull()
        {
            var userId = new Random().Next(1, 99);
            var data = new[]
            {
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
                new LocalMembership { User = new ProxiedUser(userId), },
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
            };
            data.AsEnumerable().ByUserId(data[1].User.Id, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsEnumerable().ByUserId(new Random().Next(1000, int.MaxValue), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserIdAsync_Queryable_CanAllowNull()
        {
            var userId = new Random().Next(1, 99);
            var data = new[]
            {
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
                new LocalMembership { User = new ProxiedUser(userId), },
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
            };
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByUserIdAsync(new Random().Next(1000, int.MaxValue))
                .Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserIdAsync_Queryable_CanDisallowNull()
        {
            var userId = new Random().Next(1, 99);
            var data = new[]
            {
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
                new LocalMembership { User = new ProxiedUser(userId), },
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
            };
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByUserIdAsync(data[1].User.Id, false).Result.ShouldNotBeNull();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsQueryable().ByUserIdAsync(new Random().Next(1000, int.MaxValue), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserIdAsync_Enumerable_CanAllowNull()
        {
            var userId = new Random().Next(1, 99);
            var data = new[]
            {
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
                new LocalMembership { User = new ProxiedUser(userId), },
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
            };
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByUserIdAsync(new Random().Next(1000, int.MaxValue))
                .Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserIdAsync_Enumerable_CanDisallowNull()
        {
            var userId = new Random().Next(1, 99);
            var data = new[]
            {
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
                new LocalMembership { User = new ProxiedUser(userId), },
                new LocalMembership { User = new ProxiedUser(new Random().Next(100, 999)), },
            };
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByUserIdAsync(data[1].User.Id, false).Result.ShouldNotBeNull();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByUserIdAsync(new Random().Next(1000, int.MaxValue), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        #endregion
    }
}
