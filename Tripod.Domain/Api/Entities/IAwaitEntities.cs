using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod
{
    public interface IAwaitEntities
    {
        Task<TEntity> SingleOrDefaultAsync<TEntity>(IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate) where TEntity : Entity;
    }
}
