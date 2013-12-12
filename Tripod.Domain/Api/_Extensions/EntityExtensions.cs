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

        private static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, object>> expression)
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
        #region OrderBy

        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> queryable,
            IEnumerable<KeyValuePair<Expression<Func<TEntity, object>>, OrderByDirection>> expressions)
            where TEntity : Entity
        {
            if (expressions == null) return queryable;

            // http://stackoverflow.com/a/9155222/304832
            // first expression is passed to OrderBy/Descending, others are passed to ThenBy/Descending
            var counter = 0;
            foreach (var expression in expressions)
            {
                var unaryExpression = expression.Key.Body as UnaryExpression;
                var memberExpression = unaryExpression != null ? unaryExpression.Operand as MemberExpression : null;
                var methodExpression = unaryExpression != null ? unaryExpression.Operand as MethodCallExpression : null;
                var memberOrMethodExpression = memberExpression ?? methodExpression as Expression;

                if (unaryExpression != null && memberOrMethodExpression != null)

                    if (memberOrMethodExpression.Type == typeof(DateTime))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, DateTime>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(DateTime?))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, DateTime?>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(bool))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, bool>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(bool?))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, bool?>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(int))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, int>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(int?))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, int?>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(long))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, long>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(long?))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, long?>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(short))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, short>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(short?))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, short?>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(double))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, double>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(double?))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, double?>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(float))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, float>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(float?))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, float?>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(decimal))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, decimal>>(memberOrMethodExpression, expression.Key.Parameters));

                    else if (memberOrMethodExpression.Type == typeof(decimal?))
                        queryable = queryable.ApplyOrderBy(counter, expression.Value,
                            Expression.Lambda<Func<TEntity, decimal?>>(memberOrMethodExpression, expression.Key.Parameters));

                    else
                        throw new NotSupportedException(string.Format(
                            "OrderBy object type resolution is not yet implemented for '{0}'.", memberOrMethodExpression.Type.Name));

                else
                    queryable = queryable.ApplyOrderBy(counter, expression.Value, expression.Key);

                ++counter;
            }
            return queryable;
        }

        private static IQueryable<TEntity> ApplyOrderBy<TEntity, TKey>(this IQueryable<TEntity> queryable, int counter, OrderByDirection direction, Expression<Func<TEntity, TKey>> keySelector)
            where TEntity : Entity
        {
            if (counter == 0)
            {
                queryable = direction == OrderByDirection.Ascending
                    ? queryable.OrderBy(keySelector)
                    : queryable.OrderByDescending(keySelector);
            }
            else
            {
                queryable = direction == OrderByDirection.Ascending
                    ? ((IOrderedQueryable<TEntity>)queryable).ThenBy(keySelector)
                    : ((IOrderedQueryable<TEntity>)queryable).ThenByDescending(keySelector);
            }
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

        private static Expression<Func<TEntity, bool>> ById<TEntity>(int id) where TEntity : EntityWithId<int>
        {
            return x => x.Id == id;
        }

        #endregion
    }
}
