using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod.Ioc.EntityFramework
{
    public class EntityDbContext : DbContext, ICommandEntities
    {
        #region Construction & Initialization

        public EntityDbContext()
        {
            Initializer = new BrownfieldDbInitializer();
        }

        private IDatabaseInitializer<EntityDbContext> _initializer;

        public IDatabaseInitializer<EntityDbContext> Initializer
        {
            get { return _initializer; }
            set
            {
                _initializer = value;
                Database.SetInitializer(Initializer);
            }
        }

        #endregion
        #region Model Creation

        public ICreateDbModel ModelCreator { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ModelCreator = ModelCreator ?? new DefaultDbModelCreator();
            ModelCreator.Create(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        #endregion
        #region Async

        public Task<TEntity> SingleOrDefaultAsync<TEntity>(IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate) where TEntity : Entity
        {
            return query != null ? query.SingleOrDefaultAsync(predicate) : Task.FromResult(default(TEntity));
        }

        #endregion
        #region Queries

        public IQueryable<TEntity> EagerLoad<TEntity>(IQueryable<TEntity> query,
            Expression<Func<TEntity, object>> expression) where TEntity : Entity
        {
            // Include will eager load data into the query
            if (query != null && expression != null) query = query.Include(expression);
            return query;
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : Entity
        {
            // AsNoTracking returns entities that are not attached to the DbContext
            return new EntitySet<TEntity>(Set<TEntity>().AsNoTracking(), this);
        }

        #endregion
        #region Commands

        public TEntity Get<TEntity>(object firstKeyValue, params object[] otherKeyValues) where TEntity : Entity
        {
            if (firstKeyValue == null) throw new ArgumentNullException("firstKeyValue");
            var keyValues = new List<object> { firstKeyValue };
            if (otherKeyValues != null) keyValues.AddRange(otherKeyValues);
            return Set<TEntity>().Find(keyValues.ToArray());
        }

        public IQueryable<TEntity> Get<TEntity>() where TEntity : Entity
        {
            return new EntitySet<TEntity>(Set<TEntity>(), this);
        }

        public void Create<TEntity>(TEntity entity) where TEntity : Entity
        {
            if (Entry(entity).State == EntityState.Detached) Set<TEntity>().Add(entity);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : Entity
        {
            var entry = Entry(entity);
            entry.State = EntityState.Modified;
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : Entity
        {
            if (Entry(entity).State != EntityState.Deleted)
                Set<TEntity>().Remove(entity);
        }

        public Task ReloadAsync<TEntity>(TEntity entity) where TEntity : Entity
        {
            return Entry(entity).ReloadAsync();
        }


        #endregion
        #region UnitOfWork

        public async Task DiscardChangesAsync()
        {
            foreach (var entry in ChangeTracker.Entries().Where(x => x != null))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        await entry.ReloadAsync();
                        break;
                }
            }
        }

        #endregion
    }
}
