using System.Data.Entity;
using System.Linq;
using Should;
using Should.Core.Assertions;
using Tripod.Domain.Security;

namespace Tripod.Ioc.EntityFramework
{
    public class EntityDbContextDatabaseInitializer
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
    }
}