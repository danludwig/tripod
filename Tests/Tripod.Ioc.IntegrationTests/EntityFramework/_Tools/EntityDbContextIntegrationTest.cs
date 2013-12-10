using Xunit;

namespace Tripod.Ioc.EntityFramework
{
    public abstract class EntityDbContextIntegrationTest : IUseFixture<EntityDbContextDatabaseInitializer>
    {
        void IUseFixture<EntityDbContextDatabaseInitializer>.SetFixture(EntityDbContextDatabaseInitializer initializer) { }
    }
}
