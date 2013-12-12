using System;
using System.Linq.Expressions;

namespace Tripod
{
    internal static class ExpressionExtensions
    {
        internal static Expression<Func<TEntity, bool>> Not<TEntity>(this Expression<Func<TEntity, bool>> expressionToNegate)
            where TEntity : Entity
        {
            var candidate = expressionToNegate.Parameters[0];
            var body = Expression.Not(expressionToNegate.Body);
            return Expression.Lambda<Func<TEntity, bool>>(body, candidate);
        }
    }
}
