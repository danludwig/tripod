using System.Data.Entity;

namespace Tripod.Ioc.EntityFramework
{
    public class BrownfieldDbInitializer<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        public void InitializeDatabase(TContext db)
        {
            // assume db already exists and should not be fooled with
        }
    }
}
