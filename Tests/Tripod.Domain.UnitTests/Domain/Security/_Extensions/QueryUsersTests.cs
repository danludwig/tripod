using System;
using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class QueryUsersTests
    {
        [Fact]
        public void ByName_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
            };
            data.AsQueryable().ByName(Guid.NewGuid().ToString()).ShouldBeNull();
        }

        [Fact]
        public void ByName_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
            };
            data.AsQueryable().ByName(data[0].Name, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsQueryable().ByName(Guid.NewGuid().ToString(), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture).ShouldEqual(0);
        }

        [Fact]
        public void ByName_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
            };
            data.AsEnumerable().ByName(Guid.NewGuid().ToString()).ShouldBeNull();
        }

        [Fact]
        public void ByName_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
            };
            data.AsEnumerable().ByName(data[0].Name, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsEnumerable().ByName(Guid.NewGuid().ToString(), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture).ShouldEqual(0);
        }

        [Fact]
        public void ByNameAsync_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
            };
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByNameAsync(Guid.NewGuid().ToString()).Result.ShouldBeNull();
        }

        [Fact]
        public void ByNameAsync_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
            };
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByNameAsync(data[0].Name, false).Result.ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsQueryable().ByNameAsync(Guid.NewGuid().ToString(), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture).ShouldEqual(0);
        }

        [Fact]
        public void ByNameAsync_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
            };
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByNameAsync(Guid.NewGuid().ToString()).Result.ShouldBeNull();
        }

        [Fact]
        public void ByNameAsync_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
                new User { Name = Guid.NewGuid().ToString() },
            };
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByNameAsync(data[0].Name, false).Result.ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByNameAsync(Guid.NewGuid().ToString(), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture).ShouldEqual(0);
        }
    }
}
