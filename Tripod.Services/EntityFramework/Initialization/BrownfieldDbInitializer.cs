using System.Data.Entity;

namespace Tripod.Services.EntityFramework
{
    public class BrownfieldDbInitializer : IDatabaseInitializer<DbContext>
    {
        public void InitializeDatabase(DbContext db)
        {
            // assume db already exists and should not be fooled with
        }
    }
}
