using System;
using Microsoft.AspNet.Identity;
using Should;
using Tripod.Domain.Security;
using Tripod.Ioc.EntityFramework;
using Xunit;

namespace Tripod.Ioc.Security
{
    public class SecurityStoreTests : EntityDbContextIntegrationTests
    {
        [Fact]
        public void UserLoginStoreInterface_FindAsync_FindsMatchingRemoteMemberships()
        {
            using (var dbContext = new EntityDbContext())
            {
                var user = new User { Name = Guid.NewGuid().ToString() };
                var remote1 = new RemoteMembership
                {
                    Id =
                    {
                        LoginProvider = Guid.NewGuid().ToString(),
                        ProviderKey = Guid.NewGuid().ToString()
                    }
                };
                var remote2 = new RemoteMembership
                {
                    Id =
                    {
                        LoginProvider = Guid.NewGuid().ToString(),
                        ProviderKey = Guid.NewGuid().ToString()
                    }
                };
                var remote3 = new RemoteMembership
                {
                    Id =
                    {
                        LoginProvider = Guid.NewGuid().ToString(),
                        ProviderKey = Guid.NewGuid().ToString()
                    }
                };
                user.RemoteMemberships.Add(remote1);
                user.RemoteMemberships.Add(remote2);
                user.RemoteMemberships.Add(remote3);
                dbContext.Create(user);
                dbContext.SaveChangesAsync().GetAwaiter().GetResult();

                var securityStore = new SecurityStore(dbContext);
                var result = securityStore.FindAsync(new UserLoginInfo(remote2.LoginProvider, remote2.ProviderKey)).Result;

                result.ShouldNotBeNull();
                result.ShouldEqual(user);
            }
        }

        [Fact]
        public void UserRoleStoreInterface_AddToRoleAsync_GivesUserPermission()
        {
            using (var dbContext = new EntityDbContext())
            {
                var user = new User { Name = Guid.NewGuid().ToString() };
                var permission1 = new Permission { Name = Guid.NewGuid().ToString() };
                var permission2 = new Permission { Name = Guid.NewGuid().ToString() };
                var permission3 = new Permission { Name = Guid.NewGuid().ToString() };
                dbContext.Create(user);
                dbContext.Create(permission1);
                dbContext.Create(permission2);
                dbContext.Create(permission3);
                dbContext.SaveChangesAsync().GetAwaiter().GetResult();

                var securityStore = new SecurityStore(dbContext);
                securityStore.AddToRoleAsync(user, permission2.Name).Wait();
                dbContext.SaveChangesAsync().Wait();

                user.Permissions.ShouldContain(permission2);
                permission2.Users.ShouldContain(user);
            }
        }
    }
}
