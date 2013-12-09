using Should;
using Xunit;

namespace Tripod.Ioc.EntityFramework
{
    public class BrownfieldDbInitializerTests
    {
        [Fact]
        public void InitializeDatabase_HasNoSideEffects()
        {
            var dbInitializer = new BrownfieldDbInitializer();
            var dbContext = new EntityDbContext();
            dbInitializer.InitializeDatabase(dbContext);
            dbContext.ShouldNotBeNull();
        }
    }
}
