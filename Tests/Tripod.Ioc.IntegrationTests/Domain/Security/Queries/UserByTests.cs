using System;
using System.Data.Entity;
using Should;
using Tripod.Domain.Security;
using Tripod.Ioc.EntityFramework;
using Xunit;

namespace Tripod.Ioc.Domain.Security
{
    public class UserByTests : DbContextIntegrationTest
    {
        [Fact]
        public void Handler_ReturnsUser_ById()
        {
            var dbContext = new EntityDbContext();
            var dbInitializer = new CreateDatabaseIfNotExists<EntityDbContext>();
            dbContext.Initializer = dbInitializer;
            var userName = Guid.NewGuid().ToString();
            var user = new User { Name = userName };
            dbContext.Create(user);
            var rowsAffected = dbContext.SaveChangesAsync().Result;
            var handler = new HandleUserByQuery(dbContext);

            var result = handler.Handle(new UserBy(user.Id)).Result;

            Assert.NotNull(result);
            result.ShouldEqual(user);
            rowsAffected.ShouldEqual(1);
        }

        [Fact]
        public void Handler_ReturnsUser_ByName()
        {
            var dbContext = new EntityDbContext();
            var dbInitializer = new CreateDatabaseIfNotExists<EntityDbContext>();
            dbContext.Initializer = dbInitializer;
            var userName = Guid.NewGuid().ToString();
            var user = new User { Name = userName };
            dbContext.Create(user);
            var rowsAffected = dbContext.SaveChangesAsync().Result;
            var handler = new HandleUserByQuery(dbContext);

            var result = handler.Handle(new UserBy(userName)).Result;

            Assert.NotNull(result);
            result.ShouldEqual(user);
            rowsAffected.ShouldEqual(1);
        }
    }
}
