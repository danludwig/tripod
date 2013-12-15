using Moq;
using Xunit;

namespace Tripod.Ioc.EntityFramework
{
    public class GreenfieldDbInitializerTests : EntityDbContextIntegrationTests
    {
        [Fact]
        public void InitializeDatabase_CustomizesDuringSeed_WhenDatabaseDoesNotExist()
        {
            using (var dbContext = new EntityDbContext())
            {
                dbContext.Database.Delete(); // force initializer to seed
                var dbCustomizer = new Mock<ICustomizeDb>(MockBehavior.Strict);
                dbCustomizer.Setup(x => x.Customize(It.IsAny<EntityDbContext>()));
                var dbInitializer = new GreenfieldDbInitializer(dbCustomizer.Object);
                dbContext.Initializer = dbInitializer;
                dbContext.Initializer.InitializeDatabase(dbContext);

                dbCustomizer.Verify(x => x.Customize(It.IsAny<EntityDbContext>()), Times.Once);
                dbContext.Dispose();
            }
        }
    }
}
