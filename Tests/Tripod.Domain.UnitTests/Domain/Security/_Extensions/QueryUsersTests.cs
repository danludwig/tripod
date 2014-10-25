using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class QueryUsersTests
    {
        #region ByName

        [Fact]
        public void ByName_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
            };
            data.AsQueryable().ByName(FakeData.String()).ShouldBeNull();
        }

        [Fact]
        public void ByName_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
            };
            data.AsQueryable().ByName(data[0].Name, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsQueryable().ByName(FakeData.String(), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByName_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
            };
            data.AsEnumerable().ByName(FakeData.String()).ShouldBeNull();
        }

        [Fact]
        public void ByName_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
            };
            data.AsEnumerable().ByName(data[0].Name, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsEnumerable().ByName(FakeData.String(), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByNameAsync_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
            };
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByNameAsync(FakeData.String()).Result.ShouldBeNull();
        }

        [Fact]
        public void ByNameAsync_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
            };
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByNameAsync(data[0].Name, false).Result.ShouldNotBeNull();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsQueryable().ByNameAsync(FakeData.String(), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByNameAsync_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
            };
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByNameAsync(FakeData.String()).Result.ShouldBeNull();
        }

        [Fact]
        public void ByNameAsync_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
                new User { Name = FakeData.String() },
            };
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByNameAsync(data[0].Name, false).Result.ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByNameAsync(FakeData.String(), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        #endregion
        #region ByUserLoginInfo

        [Fact]
        public void ByUserLoginInfo_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
            };
            foreach (var user in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = FakeData.String();
                    string providerKey = FakeData.String();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    user.RemoteMemberships.Add(remoteMembership);
                }
            }
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            data.AsQueryable().ByUserLoginInfo(userLoginInfo).ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfo_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
            };
            foreach (var user in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = FakeData.String();
                    string providerKey = FakeData.String();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    user.RemoteMemberships.Add(remoteMembership);
                }
            }
            var existingRemoteMembership = data[0].RemoteMemberships.First();
            var userLoginInfo = new UserLoginInfo(
                existingRemoteMembership.LoginProvider, existingRemoteMembership.ProviderKey);
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
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
            };
            foreach (var user in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = FakeData.String();
                    string providerKey = FakeData.String();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    user.RemoteMemberships.Add(remoteMembership);
                }
            }
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            data.AsEnumerable().ByUserLoginInfo(userLoginInfo).ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfo_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
            };
            foreach (var user in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = FakeData.String();
                    string providerKey = FakeData.String();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    user.RemoteMemberships.Add(remoteMembership);
                }
            }
            var existingRemoteMembership = data[0].RemoteMemberships.First();
            var userLoginInfo = new UserLoginInfo(
                existingRemoteMembership.LoginProvider, existingRemoteMembership.ProviderKey);
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
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
            };
            foreach (var user in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = FakeData.String();
                    string providerKey = FakeData.String();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    user.RemoteMemberships.Add(remoteMembership);
                }
            }
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            dbSet.Object.AsQueryable().ByUserLoginInfoAsync(userLoginInfo).Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfoAsync_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
            };
            foreach (var user in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = FakeData.String();
                    string providerKey = FakeData.String();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    user.RemoteMemberships.Add(remoteMembership);
                }
            }
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var existingRemoteMembership = data[0].RemoteMemberships.First();
            var userLoginInfo = new UserLoginInfo(
                existingRemoteMembership.LoginProvider, existingRemoteMembership.ProviderKey);
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
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
            };
            foreach (var user in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = FakeData.String();
                    string providerKey = FakeData.String();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    user.RemoteMemberships.Add(remoteMembership);
                }
            }
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            dbSet.Object.AsEnumerable().ByUserLoginInfoAsync(userLoginInfo).Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfoAsync_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
                new User { Name = FakeData.String(), },
            };
            foreach (var user in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = FakeData.String();
                    string providerKey = FakeData.String();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    user.RemoteMemberships.Add(remoteMembership);
                }
            }
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var existingRemoteMembership = data[0].RemoteMemberships.First();
            var userLoginInfo = new UserLoginInfo(
                existingRemoteMembership.LoginProvider, existingRemoteMembership.ProviderKey);
            dbSet.Object.AsEnumerable().ByUserLoginInfoAsync(userLoginInfo, false).Result.ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(FakeData.String(), FakeData.String());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByUserLoginInfoAsync(userLoginInfo, false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        #endregion
    }
}
