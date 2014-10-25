using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class QueryRemoteMembershipsTests
    {
        #region ByUserId

        [Fact]
        public void ByUserId_Queryable_FiltersByUserId()
        {
            var userId = FakeData.Id();
            var data = new[]
            {
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = userId, },
                new RemoteMembership { UserId = userId, },
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = userId, },
            };

            var results = data.AsQueryable().ByUserId(userId).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        [Fact]
        public void ByUserId_Enumerable_FiltersByUserId()
        {
            var userId = FakeData.Id();
            var data = new[]
            {
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = userId, },
                new RemoteMembership { UserId = userId, },
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = userId, },
            };

            var results = data.AsEnumerable().ByUserId(userId).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        #endregion
        #region ByUserName

        [Fact]
        public void ByUserName_Queryable_FiltersByUserName()
        {
            var userName = FakeData.String();
            var user = new User { Name = userName, };
            var data = new[]
            {
                new RemoteMembership { User = new User { Name = FakeData.String(), }, },
                new RemoteMembership { User = user, },
                new RemoteMembership { User = user, },
                new RemoteMembership { User = new User { Name = FakeData.String(), }, },
                new RemoteMembership { User = new User { Name = FakeData.String(), }, },
                new RemoteMembership { User = user, },
            };

            var results = data.AsQueryable().ByUserName(userName).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        [Fact]
        public void ByUserName_Enumerable_FiltersByUserName()
        {
            var userName = FakeData.String();
            var user = new User { Name = userName, };
            var data = new[]
            {
                new RemoteMembership { User = new User { Name = FakeData.String(), }, },
                new RemoteMembership { User = user, },
                new RemoteMembership { User = user, },
                new RemoteMembership { User = new User { Name = FakeData.String(), }, },
                new RemoteMembership { User = new User { Name = FakeData.String(), }, },
                new RemoteMembership { User = user, },
            };

            var results = data.AsEnumerable().ByUserName(userName).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        #endregion
        #region ByUserLoginInfo

        [Fact]
        public void ByUserLoginInfo_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
            };
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            data.AsQueryable().ByUserLoginInfo(userLoginInfo).ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfo_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
            };
            var existingEntity = data[0];
            var userLoginInfo = new UserLoginInfo(
                existingEntity.LoginProvider, existingEntity.ProviderKey);
            data.AsQueryable().ByUserLoginInfo(userLoginInfo, false).ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsQueryable().ByUserLoginInfo(userLoginInfo, false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserLoginInfo_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
            };
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            data.AsEnumerable().ByUserLoginInfo(userLoginInfo).ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfo_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
            };
            var existingEntity = data[0];
            var userLoginInfo = new UserLoginInfo(
                existingEntity.LoginProvider, existingEntity.ProviderKey);
            data.AsEnumerable().ByUserLoginInfo(userLoginInfo, false).ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsEnumerable().ByUserLoginInfo(userLoginInfo, false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserLoginInfoAsync_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
            };
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            dbSet.Object.AsQueryable().ByUserLoginInfoAsync(userLoginInfo).Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfoAsync_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
            };
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var existingEntity = data[0];
            var userLoginInfo = new UserLoginInfo(
                existingEntity.LoginProvider, existingEntity.ProviderKey);
            dbSet.Object.AsQueryable().ByUserLoginInfoAsync(userLoginInfo, false).Result.ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsQueryable().ByUserLoginInfoAsync(userLoginInfo, false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserLoginInfoAsync_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
            };
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            dbSet.Object.AsEnumerable().ByUserLoginInfoAsync(userLoginInfo).Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfoAsync_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()),
            };
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var existingEntity = data[0];
            var userLoginInfo = new UserLoginInfo(
                existingEntity.LoginProvider, existingEntity.ProviderKey);
            dbSet.Object.AsEnumerable().ByUserLoginInfoAsync(userLoginInfo, false).Result.ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByUserLoginInfoAsync(userLoginInfo, false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        #endregion
        #region ByUserIdAndLoginInfo

        [Fact]
        public void ByUserIdAndLoginInfo_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
            };
            var userId = FakeData.Id();
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            data.AsQueryable().ByUserIdAndLoginInfo(userId, userLoginInfo).ShouldBeNull();
        }

        [Fact]
        public void ByUserIdAndLoginInfo_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
            };
            var existingEntity = data[0];
            var userLoginInfo = new UserLoginInfo(
                existingEntity.LoginProvider, existingEntity.ProviderKey);
            data.AsQueryable().ByUserIdAndLoginInfo(existingEntity.UserId, userLoginInfo, false)
                .ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsQueryable().ByUserIdAndLoginInfo(FakeData.Id(), userLoginInfo, false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserIdAndLoginInfo_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
            };
            var userId = FakeData.Id();
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            data.AsEnumerable().ByUserIdAndLoginInfo(userId, userLoginInfo).ShouldBeNull();
        }

        [Fact]
        public void ByUserIdAndLoginInfo_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
            };
            var existingEntity = data[0];
            var userLoginInfo = new UserLoginInfo(
                existingEntity.LoginProvider, existingEntity.ProviderKey);
            data.AsEnumerable().ByUserIdAndLoginInfo(existingEntity.UserId, userLoginInfo, false)
                .ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsEnumerable().ByUserIdAndLoginInfo(FakeData.Id(), userLoginInfo, false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserIdAndLoginInfoAsync_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
            };
            var userId = FakeData.Id();
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            dbSet.Object.AsQueryable().ByUserIdAndLoginInfoAsync(userId, userLoginInfo).Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserIdAndLoginInfoAsync_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
            };
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var existingEntity = data[0];
            var userLoginInfo = new UserLoginInfo(
                existingEntity.LoginProvider, existingEntity.ProviderKey);
            dbSet.Object.AsQueryable().ByUserIdAndLoginInfoAsync(existingEntity.UserId, userLoginInfo, false)
                .Result.ShouldNotBeNull();

            var userId = FakeData.Id();
            userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsQueryable().ByUserIdAndLoginInfoAsync(userId, userLoginInfo, false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserIdAndLoginInfoAsync_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
            };
            var userId = FakeData.Id();
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            dbSet.Object.AsEnumerable().ByUserIdAndLoginInfoAsync(userId, userLoginInfo).Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserIdAndLoginInfoAsync_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = FakeData.Id()
                },
            };
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var existingEntity = data[0];
            var userLoginInfo = new UserLoginInfo(
                existingEntity.LoginProvider, existingEntity.ProviderKey);
            dbSet.Object.AsEnumerable().ByUserIdAndLoginInfoAsync(existingEntity.UserId, userLoginInfo, false)
                .Result.ShouldNotBeNull();

            var userId = FakeData.Id();
            userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByUserIdAndLoginInfoAsync(userId, userLoginInfo, false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        #endregion
    }
}
