using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Tripod.Domain.Security;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Services.Security
{
    public class SecurityStoreTests
    {
        #region IUserStore

        [Fact]
        public void UserStoreInterface_FindByIdAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.FindByIdAsync(new Random().Next(0, int.MaxValue)).Result);
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Theory, InlineData(4, false), InlineData(2, true)]
        public void UserStoreInterface_FindByIdAsync_DelegatesToDataDependency(int userId, bool expectFound)
        {
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var instance = new SecurityStore(entities.Object) as IUserStore<User, int>;
            var data = new[] { new FakeUser(1), new FakeUser(2), new FakeUser(3) }.AsQueryable();
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            entities.Setup(x => x.Get<User>()).Returns(dbSet.Object);
            Expression<Func<IQueryable<User>, bool>> expectedData = x => ReferenceEquals(x, dbSet.Object);
            entities.Setup(x => x.EagerLoad(It.Is(expectedData), It.IsAny<Expression<Func<User, object>>>())).Returns(dbSet.Object);

            var result = instance.FindByIdAsync(userId).Result;

            (result != null).ShouldEqual(expectFound);
            if (expectFound)
                result.ShouldEqual(data.Single(x => userId.Equals(x.Id)));
            entities.Verify(x => x.Get<User>(), Times.Once);
        }

        [Fact]
        public void UserStoreInterface_FindByNameAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.FindByNameAsync(Guid.NewGuid().ToString()).Result);
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Theory, InlineData("user4", false), InlineData("user2", true)]
        public void UserStoreInterface_FindByNameAsync_DelegatesToDataDependency(string userName, bool expectFound)
        {
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var instance = new SecurityStore(entities.Object) as IUserStore<User, int>;
            var data = new[]
            {
                new User { Name = "user1" },
                new User { Name = "user2" },
                new User { Name = "user3" },
            }.AsQueryable();
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            entities.Setup(x => x.Get<User>()).Returns(dbSet.Object);
            Expression<Func<IQueryable<User>, bool>> expectedData = x => ReferenceEquals(x, dbSet.Object);
            entities.Setup(x => x.EagerLoad(It.Is(expectedData), It.IsAny<Expression<Func<User, object>>>())).Returns(dbSet.Object);

            var result = instance.FindByNameAsync(userName).Result;

            (result != null).ShouldEqual(expectFound);
            if (expectFound)
                result.ShouldEqual(data.Single(x => userName.Equals(x.Name)));
            entities.Verify(x => x.Get<User>(), Times.Once);
        }

        [Fact]
        public void UserStoreInterface_CreateAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.CreateAsync(new FakeUser(6)).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserStoreInterface_CreateAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.CreateAsync(null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserStoreInterface_CreateAsync_CreatesUserEntity()
        {
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var instance = new SecurityStore(entities.Object) as IUserStore<User, int>;
            var entity = new User();
            entities.Setup(x => x.Create(entity));
            instance.CreateAsync(entity);
            entities.Verify(x => x.Create(entity), Times.Once);
        }

        [Fact]
        public void UserStoreInterface_UpdateAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.UpdateAsync(new FakeUser(6)).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserStoreInterface_UpdateAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.UpdateAsync(null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserStoreInterface_UpdateAsync_UpdatesUserEntity()
        {
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var instance = new SecurityStore(entities.Object) as IUserStore<User, int>;
            var entity = new User();
            entities.Setup(x => x.Update(entity));
            instance.UpdateAsync(entity);
            entities.Verify(x => x.Update(entity), Times.Once);
        }

        [Fact]
        public void UserStoreInterface_DeleteAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.DeleteAsync(new FakeUser(6)).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserStoreInterface_DeleteAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.DeleteAsync(null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserStoreInterface_DeleteAsync_DeletesUserEntity()
        {
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var instance = new SecurityStore(entities.Object) as IUserStore<User, int>;
            var entity = new User();
            entities.Setup(x => x.Delete(entity));
            instance.DeleteAsync(entity);
            entities.Verify(x => x.Delete(entity), Times.Once);
        }

        #endregion
        #region IQueryableUserStore

        [Fact]
        public void QueryableUserStoreInterface_UsersPropertyGetter_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IQueryableUserStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() => instance.Users);
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        #endregion
        #region IUserLoginStore

        [Fact]
        public void QueryableUserStoreInterface_UsersPropertyGetter_DelegatesToDataDependency()
        {
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var instance = new SecurityStore(entities.Object) as IQueryableUserStore<User, int>;
            var data = new[] { new FakeUser(1), new FakeUser(2), new FakeUser(3) }.AsQueryable();
            entities.Setup(x => x.Query<User>()).Returns(data);
            instance.Users.ShouldEqual(data);
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void UserLoginStoreInterface_FindAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.FindAsync(new UserLoginInfo("provider", "key")).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserLoginStoreInterface_FindAsync_ThrowsArgumentNullException_WhenUserLoginInfoIsNull()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.FindAsync(null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("login");
        }

        [Theory]
        [InlineData("provider1", "key1", true)]
        [InlineData("provider1", "key2", false)]
        [InlineData("provider2", "key1", false)]
        [InlineData("provider2", "key2", false)]
        public void UserLoginStoreInterface_FindAsync_DelegatesToDataDependency(string provider, string key, bool expectFound)
        {
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var instance = new SecurityStore(entities.Object) as IUserLoginStore<User, int>;
            var data = new[]
            {
                new User { Name = "user1" },
            }.AsQueryable();
            data.Single().RemoteMemberships.Add(new RemoteMembership
            {
                Id = { LoginProvider = Guid.NewGuid().ToString(), ProviderKey = Guid.NewGuid().ToString() }
            });
            data.Single().RemoteMemberships.Add(new RemoteMembership
            {
                Id = { LoginProvider = "provider1", ProviderKey = "key1" }
            });
            data.Single().RemoteMemberships.Add(new RemoteMembership
            {
                Id = { LoginProvider = Guid.NewGuid().ToString(), ProviderKey = Guid.NewGuid().ToString() }
            });
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            entities.Setup(x => x.Get<User>()).Returns(dbSet.Object);
            Expression<Func<IQueryable<User>, bool>> expectedData = x => ReferenceEquals(x, dbSet.Object);
            entities.Setup(x => x.EagerLoad(It.Is(expectedData), It.IsAny<Expression<Func<User, object>>>())).Returns(dbSet.Object);

            var result = instance.FindAsync(new UserLoginInfo(provider, key)).Result;

            (result != null).ShouldEqual(expectFound);
            if (expectFound)
                result.ShouldEqual(data.Single());
            entities.Verify(x => x.Get<User>(), Times.Once);
        }

        [Fact]
        public void UserLoginStoreInterface_AddLoginAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.AddLoginAsync(null, null).Wait());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserLoginStoreInterface_AddLoginAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.AddLoginAsync(null, new UserLoginInfo("provider", "key")).Wait());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserLoginStoreInterface_AddLoginAsync_ThrowsArgumentNullException_WhenUserLoginInfoIsNull()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.AddLoginAsync(new User(), null).Wait());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("login");
        }

        [Fact]
        public void UserLoginStoreInterface_AddLoginAsync_DelegatesToDataDependency()
        {
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var instance = new SecurityStore(entities.Object) as IUserLoginStore<User, int>;
            var user = new FakeUser(6);
            var login = new UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Expression<Func<RemoteMembership, bool>> expectedEntity = x => x.UserId == user.Id
                && x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey;
            entities.Setup(x => x.Create(It.Is(expectedEntity)));
            instance.AddLoginAsync(user, login);
            entities.Verify(x => x.Create(It.Is(expectedEntity)), Times.Once);
        }

        [Fact]
        public void UserLoginStoreInterface_RemoveLoginAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.RemoveLoginAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserLoginStoreInterface_RemoveLoginAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.RemoveLoginAsync(null, new UserLoginInfo("provider", "key")).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserLoginStoreInterface_RemoveLoginAsync_ThrowsArgumentNullException_WhenUserLoginInfoIsNull()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.RemoveLoginAsync(new User(), null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("login");
        }

        [Theory, InlineData("provider1", "key1", true), InlineData("provider2", "key2", false)]
        public void UserLoginStoreInterface_RemoveLoginAsync_DelegatesToUserCollectionProperty(string provider, string key, bool expectRemove)
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            var user = new FakeUser(6);
            user.RemoteMemberships.Add(new RemoteMembership
            {
                UserId = user.Id,
                Id = { LoginProvider = "provider1", ProviderKey = "key1" }
            });
            var login = new UserLoginInfo(provider, key);
            instance.RemoveLoginAsync(user, login).Wait();

            user.RemoteMemberships.Any(x => x.LoginProvider == provider && x.ProviderKey == provider).ShouldEqual(false);
            user.RemoteMemberships.Any().ShouldEqual(!expectRemove);
        }

        [Fact]
        public void UserLoginStoreInterface_GetLoginsAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.GetLoginsAsync(null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserLoginStoreInterface_GetLoginsAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.GetLoginsAsync(null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserLoginStoreInterface_GetLoginsAsync_DelegatesToUserCollectionProperty()
        {
            var instance = new SecurityStore(null) as IUserLoginStore<User, int>;
            var user = new FakeUser(6);
            user.RemoteMemberships.Add(new RemoteMembership
            {
                UserId = user.Id,
                Id = { LoginProvider = "provider1", ProviderKey = "key1" }
            });
            user.RemoteMemberships.Add(new RemoteMembership
            {
                UserId = user.Id,
                Id = { LoginProvider = "provider2", ProviderKey = "key2" }
            });
            var result = instance.GetLoginsAsync(user).Result;

            result.ShouldNotBeNull();
            result.Count.ShouldEqual(2);
            result.Any(x => x.LoginProvider == user.RemoteMemberships.ElementAt(0).LoginProvider
                && x.ProviderKey == user.RemoteMemberships.ElementAt(0).ProviderKey).ShouldBeTrue();
            result.Any(x => x.LoginProvider == user.RemoteMemberships.ElementAt(1).LoginProvider
                && x.ProviderKey == user.RemoteMemberships.ElementAt(1).ProviderKey).ShouldBeTrue();
        }

        #endregion
        #region IUserRoleStore

        [Fact]
        public void UserRoleStoreInterface_AddToRoleAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.AddToRoleAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserRoleStoreInterface_AddToRoleAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.AddToRoleAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Theory, InlineData(null), InlineData(""), InlineData("\t   \r\n")]
        public void UserRoleStoreInterface_AddToRoleAsync_ThrowsArgumentException_WhenRoleIsNullOrWhiteSpace(string roleName)
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            var exception = Assert.Throws<ArgumentException>(() =>
                instance.AddToRoleAsync(new User(), roleName).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("roleName");
            exception.Message.ShouldStartWith(Resources.Exception_Argument_CannotBeNullOrEmpty);
        }

        [Fact]
        public void UserRoleStoreInterface_AddToRoleAsync_ThrowsInvalidOperation_WhenRoleNameDoesNotExist()
        {
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var instance = new SecurityStore(entities.Object) as IUserRoleStore<User, int>;
            const string roleName = "test";
            var data = new[]
            {
                new Permission { Name = Guid.NewGuid().ToString() },
                new Permission { Name = Guid.NewGuid().ToString() },
                new Permission { Name = Guid.NewGuid().ToString() },
            }.AsQueryable();
            var dbSet = new Mock<DbSet<Permission>>(MockBehavior.Strict).SetupDataAsync(data);
            entities.Setup(x => x.Get<Permission>()).Returns(dbSet.Object);
            var exception = Assert.Throws<InvalidOperationException>(() =>
                instance.AddToRoleAsync(new User(), roleName).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.Message.ShouldEqual(string.Format(Resources.Exception_InvalidOperation_DoesNotExist,
                Permission.Constraints.Label, roleName));
        }

        [Fact]
        public void UserRoleStoreInterface_AddToRoleAsync_AddsInstancesToCollectionProperties_ForBothSidesOfEntityRelationship()
        {
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var instance = new SecurityStore(entities.Object) as IUserRoleStore<User, int>;
            const string roleName = "test";
            var user = new User();
            var data = new[]
            {
                new Permission { Name = Guid.NewGuid().ToString() },
                new Permission { Name = roleName },
                new Permission { Name = Guid.NewGuid().ToString() },
            }.AsQueryable();
            var dbSet = new Mock<DbSet<Permission>>(MockBehavior.Strict).SetupDataAsync(data);
            entities.Setup(x => x.Get<Permission>()).Returns(dbSet.Object);

            instance.AddToRoleAsync(user, roleName).Wait();

            user.Permissions.ShouldContain(data.ElementAt(1));
            data.ElementAt(1).Users.ShouldContain(user);
        }

        [Fact]
        public void UserRoleStoreInterface_RemoveFromRoleAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.RemoveFromRoleAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserRoleStoreInterface_RemoveFromRoleAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.RemoveFromRoleAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Theory, InlineData(null), InlineData(""), InlineData("\t   \r\n")]
        public void UserRoleStoreInterface_RemoveFromRoleAsync_ThrowsArgumentException_WhenRoleIsNullOrWhiteSpace(string roleName)
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            var exception = Assert.Throws<ArgumentException>(() =>
                instance.RemoveFromRoleAsync(new User(), roleName).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("roleName");
            exception.Message.ShouldStartWith(Resources.Exception_Argument_CannotBeNullOrEmpty);
        }

        [Fact]
        public void UserRoleStoreInterface_RemoveFromRoleAsync_DoesNothing_WhenUserIsNotInRole()
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            var user = new User();
            user.Permissions.Add(new Permission { Name = "test1" });
            instance.RemoveFromRoleAsync(new User(), "test2");
            user.Permissions.Count.ShouldEqual(1);
        }

        [Fact]
        public void UserRoleStoreInterface_RemoveFromRoleAsync_RemotesInstancesFromCollectionProperties_ForBothSidesOfEntityRelationship()
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            const string roleName = "test";
            var user = new User();
            var permission = new Permission { Name = roleName };
            user.Permissions.Add(permission);
            permission.Users.Add(user);

            instance.RemoveFromRoleAsync(user, roleName).Wait();

            user.Permissions.ShouldNotContain(permission);
            permission.Users.ShouldNotContain(user);
        }

        [Fact]
        public void UserRoleStoreInterface_GetRolesAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.GetRolesAsync(null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserRoleStoreInterface_GetRolesAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.GetRolesAsync(null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Theory, InlineData("role1", true), InlineData("role4", false)]
        public void UserRoleStoreInterface_GetRolesAsync_DelegatesToUserCollection(string roleName, bool expectInResult)
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            var user = new User();
            user.Permissions.Add(new Permission { Name = "role1" });
            user.Permissions.Add(new Permission { Name = "role2" });
            user.Permissions.Add(new Permission { Name = "role3" });

            var result = instance.GetRolesAsync(user).Result;
            result.Count.ShouldEqual(3);
            if (expectInResult) result.ShouldContain(roleName);
            else result.ShouldNotContain(roleName);
        }

        [Fact]
        public void UserRoleStoreInterface_IsInRoleAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.IsInRoleAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserRoleStoreInterface_IsInRoleAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.IsInRoleAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Theory, InlineData(null), InlineData(""), InlineData("\t   \r\n")]
        public void UserRoleStoreInterface_IsInRoleAsync_ThrowsArgumentException_WhenRoleIsNullOrWhiteSpace(string roleName)
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            var exception = Assert.Throws<ArgumentException>(() =>
                instance.IsInRoleAsync(new User(), roleName).Result);
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("roleName");
            exception.Message.ShouldStartWith(Resources.Exception_Argument_CannotBeNullOrEmpty);
        }

        [Theory, InlineData("role1", true), InlineData("role4", false)]
        public void UserRoleStoreInterface_IsInRoleAsync_DelegatesToUserCollection(string roleName, bool expect)
        {
            var instance = new SecurityStore(null) as IUserRoleStore<User, int>;
            var user = new User();
            user.Permissions.Add(new Permission { Name = "role1" });
            user.Permissions.Add(new Permission { Name = "role2" });
            user.Permissions.Add(new Permission { Name = "role3" });

            var result = instance.IsInRoleAsync(user, roleName).Result;
            result.ShouldEqual(expect);
        }

        #endregion
        #region IUserPasswordStore

        [Fact]
        public void UserPasswordStoreInterface_SetPasswordHashAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserPasswordStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.SetPasswordHashAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserPasswordStoreInterface_SetPasswordHashAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserPasswordStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.SetPasswordHashAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserPasswordStoreInterface_SetPasswordHashAsync_CreatesLocalMembership_WhenUserDoesNotHaveOne()
        {
            var instance = new SecurityStore(null) as IUserPasswordStore<User, int>;
            var user = new User();
            var passwordHash = Guid.NewGuid().ToString();
            instance.SetPasswordHashAsync(user, passwordHash).Wait();

            user.LocalMembership.ShouldNotBeNull();
            user.LocalMembership.PasswordHash.ShouldEqual(passwordHash);
        }

        [Fact]
        public void UserPasswordStoreInterface_SetPasswordHashAsync_DelegatesToUserLocalMembership()
        {
            var instance = new SecurityStore(null) as IUserPasswordStore<User, int>;
            var user = new User
            {
                LocalMembership = new LocalMembership()
            };
            var passwordHash = Guid.NewGuid().ToString();
            instance.SetPasswordHashAsync(user, passwordHash).Wait();

            user.LocalMembership.PasswordHash.ShouldEqual(passwordHash);
        }

        [Fact]
        public void UserPasswordStoreInterface_GetPasswordHashAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserPasswordStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.GetPasswordHashAsync(null).Result);
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserPasswordStoreInterface_GetPasswordHashAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserPasswordStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.GetPasswordHashAsync(null).Result);
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserPasswordStoreInterface_GetPasswordHashAsync_ReturnsNull_WhenUserHasNoLocalMembership()
        {
            var instance = new SecurityStore(null) as IUserPasswordStore<User, int>;
            var user = new User();
            var result = instance.GetPasswordHashAsync(user).Result;
            result.ShouldBeNull();
        }

        [Fact]
        public void UserPasswordStoreInterface_GetPasswordHashAsync_DelegatesToUserLocalMembershipProperty()
        {
            var instance = new SecurityStore(null) as IUserPasswordStore<User, int>;
            var user = new User();
            var localMembership = new LocalMembership { PasswordHash = Guid.NewGuid().ToString() };
            user.LocalMembership = localMembership;
            var result = instance.GetPasswordHashAsync(user).Result;
            result.ShouldEqual(localMembership.PasswordHash);
            result.ShouldEqual(user.LocalMembership.PasswordHash);
        }

        [Fact]
        public void UserPasswordStoreInterface_HasPasswordAsync_ReturnsFalse_WhenUserHasNoLocalMembership()
        {
            var instance = new SecurityStore(null) as IUserPasswordStore<User, int>;
            var user = new User();
            var result = instance.HasPasswordAsync(user).Result;
            result.ShouldBeFalse();
        }

        [Theory, InlineData("hashedPassword", true), InlineData(null, false)]
        public void UserPasswordStoreInterface_HasPasswordAsync_DelegatesToUserLocalMembershipProperty(string passwordHash, bool expect)
        {
            var instance = new SecurityStore(null) as IUserPasswordStore<User, int>;
            var user = new User();
            var localMembership = new LocalMembership { PasswordHash = passwordHash };
            user.LocalMembership = localMembership;
            var result = instance.HasPasswordAsync(user).Result;
            result.ShouldEqual(expect);
        }

        #endregion
        #region IUserClaimStore

        [Fact]
        public void UserClaimStoreInterface_GetClaimsAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.GetClaimsAsync(null).Result);
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserClaimStoreInterface_GetClaimsAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.GetClaimsAsync(null).Result);
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Theory, InlineData("type1", "value1", true), InlineData("type4", "value4", false)]
        public void UserClaimStoreInterface_GetClaimsAsync_DelegatesToUserPropertyCollection(string claimType, string claimValue, bool expectInResult)
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            var user = new User();
            user.Claims.Add(new UserClaim { ClaimType = "type1", ClaimValue = "value1" });
            user.Claims.Add(new UserClaim { ClaimType = "type2", ClaimValue = "value2" });
            user.Claims.Add(new UserClaim { ClaimType = "type3", ClaimValue = "value3" });
            var result = instance.GetClaimsAsync(user).Result;
            result.ShouldNotBeNull();
            result.Count.ShouldEqual(3);
            result.Any(x => x.Type == claimType && x.Value == claimValue).ShouldEqual(expectInResult);
        }

        [Fact]
        public void UserClaimStoreInterface_AddClaimAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.AddClaimAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserClaimStoreInterface_AddClaimAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.AddClaimAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserClaimStoreInterface_AddClaimAsync_ThrowsArgumentNullException_WhenClaimIsNull()
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.AddClaimAsync(new User(), null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("claim");
        }

        [Fact]
        public void UserClaimStoreInterface_AddClaimAsync_AddsClaimToUserCollectionProperty()
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            var user = new User();
            var claim = new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            instance.AddClaimAsync(user, claim).Wait();
            user.Claims.Count.ShouldEqual(1);
            user.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value).ShouldBeTrue();
        }

        [Fact]
        public void UserClaimStoreInterface_RemoveClaimAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.RemoveClaimAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserClaimStoreInterface_RemoveClaimAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.RemoveClaimAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserClaimStoreInterface_RemoveClaimAsync_ThrowsArgumentNullException_WhenClaimIsNull()
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.RemoveClaimAsync(new User(), null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("claim");
        }

        [Fact]
        public void UserClaimStoreInterface_RemoveClaimAsync_RemovesClaimFromUserCollectionProperty()
        {
            var instance = new SecurityStore(null) as IUserClaimStore<User, int>;
            var user = new User();
            var securityClaim = new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            user.Claims.Add(new UserClaim { ClaimType = securityClaim.Type, ClaimValue = securityClaim.Value });
            user.Claims.Add(new UserClaim { ClaimType = Guid.NewGuid().ToString(), ClaimValue = Guid.NewGuid().ToString() });
            user.Claims.Add(new UserClaim { ClaimType = Guid.NewGuid().ToString(), ClaimValue = Guid.NewGuid().ToString() });
            instance.RemoveClaimAsync(user, securityClaim);
            user.Claims.Count.ShouldEqual(2);
            user.Claims.Any(x => x.ClaimType == securityClaim.Type && x.ClaimValue == securityClaim.Value).ShouldBeFalse();
        }

        #endregion
        #region ISecurityStampStore

        [Fact]
        public void UserSecurityStampStoreInterface_SetSecurityStampAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserSecurityStampStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.SetSecurityStampAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserSecurityStampStoreInterface_SetSecurityStampAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserSecurityStampStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.SetSecurityStampAsync(null, null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserSecurityStampStoreInterface_SetSecurityStampAsync_SetsUserProperty()
        {
            var instance = new SecurityStore(null) as IUserSecurityStampStore<User, int>;
            var user = new User();
            var stamp = Guid.NewGuid().ToString();
            instance.SetSecurityStampAsync(user, stamp).Wait();
            user.SecurityStamp.ShouldEqual(stamp);
        }

        [Fact]
        public void UserSecurityStampStoreInterface_GetSecurityStampAsync_ThrowsWhenDisposed()
        {
            var instance = new SecurityStore(null) as IUserSecurityStampStore<User, int>;
            instance.Dispose();
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                instance.GetSecurityStampAsync(null).Result);
            exception.ShouldNotBeNull();
            exception.ObjectName.ShouldEqual(instance.GetType().Name);
        }

        [Fact]
        public void UserSecurityStampStoreInterface_GetSecurityStampAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var instance = new SecurityStore(null) as IUserSecurityStampStore<User, int>;
            var exception = Assert.Throws<ArgumentNullException>(() =>
                instance.GetSecurityStampAsync(null).Result);
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Fact]
        public void UserSecurityStampStoreInterface_GetSecurityStampAsync_GetsUserProperty()
        {
            var instance = new SecurityStore(null) as IUserSecurityStampStore<User, int>;
            var user = new User();
            var stamp = Guid.NewGuid().ToString();
            user.SecurityStamp = stamp;
            var result = instance.GetSecurityStampAsync(user).Result;
            result.ShouldEqual(stamp);
        }

        #endregion
    }
}
