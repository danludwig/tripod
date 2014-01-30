using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod
{
    public static class EntityExtensions
    {
        #region EagerLoad

        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, object>> expression)
            where TEntity : Entity
        {
            var set = queryable as EntitySet<TEntity>;
            if (set != null)
                set.Queryable = set.Entities.EagerLoad(set.Queryable, expression);
            return queryable;
        }

        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> queryable, IEnumerable<Expression<Func<TEntity, object>>> expressions)
            where TEntity : Entity
        {
            if (expressions != null)
                queryable = expressions.Aggregate(queryable, (current, expression) => current.EagerLoad(expression));
            return queryable;
        }

        #endregion
        #region ById (int)

        public static TEntity ById<TEntity>(this IQueryable<TEntity> set, int id, bool allowNull = true) where TEntity : EntityWithId<int>
        {
            return allowNull ? set.SingleOrDefault(ById<TEntity>(id)) : set.Single(ById<TEntity>(id));
        }

        public static TEntity ById<TEntity>(this IEnumerable<TEntity> set, int id, bool allowNull = true) where TEntity : EntityWithId<int>
        {
            return set.AsQueryable().ById(id, allowNull);
        }

        public static Task<TEntity> ByIdAsync<TEntity>(this IQueryable<TEntity> set, int id, bool allowNull = true) where TEntity : EntityWithId<int>
        {
            return allowNull ? set.SingleOrDefaultAsync(ById<TEntity>(id)) : set.SingleAsync(ById<TEntity>(id));
        }

        public static Task<TEntity> ByIdAsync<TEntity>(this IEnumerable<TEntity> set, int id, bool allowNull = true) where TEntity : EntityWithId<int>
        {
            return set.AsQueryable().ByIdAsync(id, allowNull);
        }

        internal static Expression<Func<TEntity, bool>> ById<TEntity>(int id) where TEntity : EntityWithId<int>
        {
            return x => x.Id == id;
        }

        #endregion
    }
}
