using System;
using System.Linq;
using System.Linq.Expressions;

namespace Tripod
{
    /// <summary>
    /// Informs an underlying relational data store to return sets of read-only entity instances.
    /// </summary>
    public interface IQueryEntities
    {
        /// <summary>
        /// Inform an underlying relational data store to return a set of read-only entity instances.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity instances that the underlying relational data
        /// store should return.</typeparam>
        /// <returns>IQueryable for set of read-only TEntity instances from an underlying relational
        /// data store.</returns>
        IQueryable<TEntity> Query<TEntity>() where TEntity : Entity;

        /// <summary>
        /// Inform an underlying relational data store to return entity instances along with related
        /// entity instances in one step.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity instances that the underlying relational data
        /// store should return.</typeparam>
        /// <param name="query">Query for  a set of TEntity instances from an underlying relational
        /// data store.</param>
        /// <param name="expression">Related entity instance set(s) that the underlying relational
        /// data store should return along with the set of TEntity instances.</param>
        /// <returns>IQueryable for set of TEntity instances informing the underlying relational data
        /// store to also return related entities identified in the expression argument.</returns>
        IQueryable<TEntity> EagerLoad<TEntity>(IQueryable<TEntity> query, Expression<Func<TEntity, object>> expression)
            where TEntity : Entity;
    }
}
