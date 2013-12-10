using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod
{
    public interface IAwaitQueryables
    {
        Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
    }
}
