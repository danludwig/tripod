using System;
using System.Data.Entity;
using System.Linq;
using Should;
using Should.Core.Assertions;
using Tripod.Domain.Security;

namespace Tripod.Services.EntityFramework
{
    public class EntityDbContextDatabaseInitializer : IDisposable
    {
        public EntityDbContextDatabaseInitializer()
        {
            var dbContext = new EntityDbContext
            {
                Initializer = new DropCreateDatabaseIfModelChanges<EntityDbContext>()
            };
            dbContext.Initializer.InitializeDatabase(dbContext);
            var users = dbContext.Set<User>().ToArray();
            Assert.NotNull(users);
            users.Count().ShouldBeInRange(0, int.MaxValue);
            dbContext.Dispose();
        }

        void IDisposable.Dispose()
        {
            using (var dbContext = new EntityDbContext())
            {
                dbContext.Database.Delete();
            }
        }
    }
}