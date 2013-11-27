namespace Tripod.Ioc.EntityFramework
{
    public class EntityDbContextDatabasePurger : DbContextDatabasePurger<EntityDbContext>
    {
        public EntityDbContextDatabasePurger()
        {
            if (DbContext == null) DbContext = new EntityDbContext();
        }
    }
}