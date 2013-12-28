using System.Linq;
using System.Threading.Tasks;

namespace Tripod
{
    /// <summary>
    /// Informs an underlying relational data store to accept or return sets of writeable entity instances.
    /// </summary>
    public interface IWriteEntities : IUnitOfWork, IReadEntities
    {
        /// <summary>
        /// Inform an underlying relational data store to return a single writable entity instance.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity instance that the underlying relational data store
        /// should return.</typeparam>
        /// <param name="firstKeyValue">First or only primary key value of the entity instance that the
        /// underlying relational data store should return.</param>
        /// <param name="otherKeyValues">Other components of a composite primary key not identified in the
        /// firstKeyValue argument.</param>
        /// <returns>A single writable entity instance whose primary key matches the argument value(s), if one
        /// exists in the underlying relational data store.
        /// Otherwise, null.</returns>
        TEntity Get<TEntity>(object firstKeyValue, params object[] otherKeyValues) where TEntity : Entity;

        /// <summary>
        /// Asynchronously inform an underlying relational data store to return a single writable entity instance.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity instance that the underlying relational data store
        /// should return.</typeparam>
        /// <param name="firstKeyValue">First or only primary key value of the entity instance that the
        /// underlying relational data store should return.</param>
        /// <param name="otherKeyValues">Other components of a composite primary key not identified in the
        /// firstKeyValue argument.</param>
        /// <returns>A task result containing a single writable entity instance whose primary key matches the argument value(s),
        /// if one exists in the underlying relational data store. Otherwise, the task result will represent null.</returns>
        Task<TEntity> GetAsync<TEntity>(object firstKeyValue, params object[] otherKeyValues) where TEntity : Entity;

        /// <summary>
        /// Inform an underlying relational data store to return a set of writable entity instances.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity instances that the underlying relational data store
        /// should return.</typeparam>
        /// <returns>IQueryable for set of writable TEntity instances from an underlying relational data
        /// store.</returns>
        IQueryable<TEntity> Get<TEntity>() where TEntity : Entity;

        /// <summary>
        /// Inform the underlying relational data store that a new entity instance should be added to a set
        /// of entity instances.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity instance set that the underlying relational data
        /// store should add to the entity instance to.</typeparam>
        /// <param name="entity">Entity instance that should be added to the TEntity set by the underlying
        /// relational data store.</param>
        void Create<TEntity>(TEntity entity) where TEntity : Entity;

        /// <summary>
        /// Inform the underlying relational data store that an existing entity instance should be
        /// permanently removed from its set of entity instances.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity instance set that the underlying relational data
        /// store should permanently remove the entity instance from.</typeparam>
        /// <param name="entity">Entity instance that should be permanently removed from the TEntity set by
        /// the underlying relational data store.</param>
        void Delete<TEntity>(TEntity entity) where TEntity : Entity;

        /// <summary>
        /// Inform the underlying relational data store that an existing entity instance's data state may
        /// have changed.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity instance set that the changed entity instance
        /// is part of.</typeparam>
        /// <param name="entity">Entity instance whose data state may be different from that of the
        /// underlying relational data store.</param>
        void Update<TEntity>(TEntity entity) where TEntity : Entity;

        /// <summary>
        /// Inform the underlying relational data store to replace the data state of an entity instance with
        /// the values currently saved in the underlying relational data store.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity instance set that the entity instance to be reloaded
        /// is part of.</typeparam>
        /// <param name="entity">Entity instance whose data state will be replaced using the values currently
        /// saved in the underlying relational data store.</param>
        void Reload<TEntity>(TEntity entity) where TEntity : Entity;

        /// <summary>
        /// Asynchronously inform the underlying relational data store to replace the data state of an entity instance with
        /// the values currently saved in the underlying relational data store.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity instance set that the entity instance to be reloaded
        /// is part of.</typeparam>
        /// <param name="entity">Entity instance whose data state will be replaced using the values currently
        /// saved in the underlying relational data store.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ReloadAsync<TEntity>(TEntity entity) where TEntity : Entity;
    }
}
