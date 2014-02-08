using Xunit;

namespace Tripod.Services.EntityFramework
{
    public abstract class EntityDbContextIntegrationTests : IUseFixture<EntityDbContextDatabaseInitializer>
    {
        void IUseFixture<EntityDbContextDatabaseInitializer>.SetFixture(EntityDbContextDatabaseInitializer initializer) { }
    }
}
