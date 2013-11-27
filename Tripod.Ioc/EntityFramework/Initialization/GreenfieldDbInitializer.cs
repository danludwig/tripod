using System.Data.Entity;

namespace Tripod.Ioc.EntityFramework
{
    public class GreenfieldDbInitializer : DropCreateDatabaseIfModelChanges<DbContext>
    {
        private readonly ICustomizeDb _customizer;

        public GreenfieldDbInitializer(ICustomizeDb customizer)
        {
            _customizer = customizer;
        }

        protected override void Seed(DbContext db)
        {
            if (_customizer != null) _customizer.Customize(db);
        }
    }
}
