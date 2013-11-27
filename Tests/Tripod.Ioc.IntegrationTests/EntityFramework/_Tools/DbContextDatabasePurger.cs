using System;
using System.Data.Entity;

namespace Tripod.Ioc.EntityFramework
{
    public class DbContextDatabasePurger<TContext> : IDisposable where TContext : DbContext
    {
        void IDisposable.Dispose()
        {
            DbContext.Database.Delete();
        }

        protected TContext DbContext { get; set; }
    }
}