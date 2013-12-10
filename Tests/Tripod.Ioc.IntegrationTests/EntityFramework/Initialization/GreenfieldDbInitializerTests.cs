using Moq;
using Xunit;

namespace Tripod.Ioc.EntityFramework
{
    public class GreenfieldDbInitializerTests : EntityDbContextIntegrationTest
    {
        [Fact]
        public void InitializeDatabase_CustomizesDuringSeed_WhenDatabaseDoesNotExist()
        {
            var dbContext = new EntityDbContext();
            dbContext.Database.Delete(); // force initializer to seed
            var dbCustomizer = new Mock<ICustomizeDb>(MockBehavior.Strict);
            dbCustomizer.Setup(x => x.Customize(dbContext));
            var dbInitializer = new GreenfieldDbInitializer(dbCustomizer.Object);

            dbInitializer.InitializeDatabase(dbContext);
            
            dbCustomizer.Verify(x => x.Customize(dbContext), Times.Once());
        }
    }
}
