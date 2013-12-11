using Xunit;

namespace Tripod.Ioc.EntityFramework
{
    public abstract class EntityDbContextIntegrationTests : IUseFixture<EntityDbContextDatabaseInitializer>
    {
        void IUseFixture<EntityDbContextDatabaseInitializer>.SetFixture(EntityDbContextDatabaseInitializer initializer) { }
    }
}
