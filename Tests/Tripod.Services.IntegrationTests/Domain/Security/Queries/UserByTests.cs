using System;
using System.Linq.Expressions;
using Should;
using Tripod.Domain.Security;
using Tripod.Services.EntityFramework;
using Xunit;

namespace Tripod.Services.Domain.Security
{
    public class UserByTests : EntityDbContextIntegrationTests
    {
        [Fact]
        public void Handler_ReturnsUser_ById()
        {
            using (var dbContext = new EntityDbContext())
            {
                var userName = Guid.NewGuid().ToString();
                var user = new User { Name = userName };
                dbContext.Create(user);
                var rowsAffected = dbContext.SaveChangesAsync().Result;
                var handler = new HandleUserByQuery(dbContext);

                var result = handler.Handle(new UserBy(user.Id)
                {
                    EagerLoad = new Expression<Func<User, object>>[]
                    {
                        x => x.Permissions,
                    }
                }).Result;

                Assert.NotNull(result);
                result.ShouldEqual(user);
                rowsAffected.ShouldEqual(1);
            }
        }

        [Fact]
        public void Handler_ReturnsUser_ByName_CaseInsensitively()
        {
            using (var dbContext = new EntityDbContext())
            {
                var userName = Guid.NewGuid().ToString().ToUpper();
                var user = new User { Name = userName };
                dbContext.Create(user);
                var rowsAffected = dbContext.SaveChangesAsync().Result;
                var handler = new HandleUserByQuery(dbContext);

                var result = handler.Handle(new UserBy(userName.ToLower())).Result;

                Assert.NotNull(result);
                result.ShouldEqual(user);
                rowsAffected.ShouldEqual(1);
            }
        }
    }
}
