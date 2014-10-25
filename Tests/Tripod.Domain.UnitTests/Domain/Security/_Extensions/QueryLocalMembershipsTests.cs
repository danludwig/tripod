﻿using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
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
        #region ByUserName

        [Fact]
        public void ByUserName_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            data.AsQueryable().ByUserName(Guid.NewGuid().ToString()).ShouldBeNull();
        }

        [Fact]
        public void ByUserName_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            data.AsQueryable().ByUserName(data[0].User.Name, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsQueryable().ByUserName(Guid.NewGuid().ToString(), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserName_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            data.AsEnumerable().ByUserName(Guid.NewGuid().ToString()).ShouldBeNull();
        }

        [Fact]
        public void ByUserName_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            data.AsEnumerable().ByUserName(data[0].User.Name, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsEnumerable().ByUserName(Guid.NewGuid().ToString(), false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserNameAsync_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByUserNameAsync(Guid.NewGuid().ToString())
                .Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserNameAsync_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByUserNameAsync(data[0].User.Name, false).Result.ShouldNotBeNull();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsQueryable().ByUserNameAsync(Guid.NewGuid().ToString(), false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByUserNameAsync_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByUserNameAsync(Guid.NewGuid().ToString())
                .Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserNameAsync_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByUserNameAsync(data[0].User.Name, false).Result.ShouldNotBeNull();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByUserNameAsync(Guid.NewGuid().ToString(), false).Result);
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
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            foreach (var localMembership in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = Guid.NewGuid().ToString();
                    string providerKey = Guid.NewGuid().ToString();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    localMembership.User.RemoteMemberships.Add(remoteMembership);
                }
            }
            var userLoginInfo = new UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            data.AsQueryable().ByUserLoginInfo(userLoginInfo).ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfo_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            foreach (var localMembership in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = Guid.NewGuid().ToString();
                    string providerKey = Guid.NewGuid().ToString();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    localMembership.User.RemoteMemberships.Add(remoteMembership);
                }
            }
            var existingRemoteMembership = data[0].User.RemoteMemberships.First();
            var userLoginInfo = new UserLoginInfo(
                existingRemoteMembership.LoginProvider, existingRemoteMembership.ProviderKey);
            data.AsQueryable().ByUserLoginInfo(userLoginInfo, false).ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
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
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            foreach (var localMembership in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = Guid.NewGuid().ToString();
                    string providerKey = Guid.NewGuid().ToString();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    localMembership.User.RemoteMemberships.Add(remoteMembership);
                }
            }
            var userLoginInfo = new UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            data.AsEnumerable().ByUserLoginInfo(userLoginInfo).ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfo_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            foreach (var localMembership in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = Guid.NewGuid().ToString();
                    string providerKey = Guid.NewGuid().ToString();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    localMembership.User.RemoteMemberships.Add(remoteMembership);
                }
            }
            var existingRemoteMembership = data[0].User.RemoteMemberships.First();
            var userLoginInfo = new UserLoginInfo(
                existingRemoteMembership.LoginProvider, existingRemoteMembership.ProviderKey);
            data.AsEnumerable().ByUserLoginInfo(userLoginInfo, false).ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
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
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            foreach (var localMembership in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = Guid.NewGuid().ToString();
                    string providerKey = Guid.NewGuid().ToString();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    localMembership.User.RemoteMemberships.Add(remoteMembership);
                }
            }
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var userLoginInfo = new UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            dbSet.Object.AsQueryable().ByUserLoginInfoAsync(userLoginInfo).Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfoAsync_Queryable_CanDisallowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            foreach (var localMembership in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = Guid.NewGuid().ToString();
                    string providerKey = Guid.NewGuid().ToString();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    localMembership.User.RemoteMemberships.Add(remoteMembership);
                }
            }
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var existingRemoteMembership = data[0].User.RemoteMemberships.First();
            var userLoginInfo = new UserLoginInfo(
                existingRemoteMembership.LoginProvider, existingRemoteMembership.ProviderKey);
            dbSet.Object.AsQueryable().ByUserLoginInfoAsync(userLoginInfo, false).Result.ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
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
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            foreach (var localMembership in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = Guid.NewGuid().ToString();
                    string providerKey = Guid.NewGuid().ToString();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    localMembership.User.RemoteMemberships.Add(remoteMembership);
                }
            }
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var userLoginInfo = new UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            dbSet.Object.AsEnumerable().ByUserLoginInfoAsync(userLoginInfo).Result.ShouldBeNull();
        }

        [Fact]
        public void ByUserLoginInfoAsync_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
                new LocalMembership { User = new User { Name = Guid.NewGuid().ToString() }, },
            };
            foreach (var localMembership in data)
            {
                for (var i = 0; i < 3; i++)
                {
                    string loginProvider = Guid.NewGuid().ToString();
                    string providerKey = Guid.NewGuid().ToString();
                    var remoteMembership = new ProxiedRemoteMembership(loginProvider, providerKey);
                    localMembership.User.RemoteMemberships.Add(remoteMembership);
                }
            }
            var dbSet = new Mock<DbSet<LocalMembership>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var existingRemoteMembership = data[0].User.RemoteMemberships.First();
            var userLoginInfo = new UserLoginInfo(
                existingRemoteMembership.LoginProvider, existingRemoteMembership.ProviderKey);
            dbSet.Object.AsEnumerable().ByUserLoginInfoAsync(userLoginInfo, false).Result.ShouldNotBeNull();

            userLoginInfo = new UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByUserLoginInfoAsync(userLoginInfo, false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        #endregion
    }
}
