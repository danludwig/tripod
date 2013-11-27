using Xunit;

namespace Tripod.Ioc.EntityFramework
{
    public abstract class DbContextIntegrationTest : IUseFixture<EntityDbContextDatabasePurger>
    {
        void IUseFixture<EntityDbContextDatabasePurger>.SetFixture(EntityDbContextDatabasePurger purger) { }
    }
}
