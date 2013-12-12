using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Moq;

namespace Tripod
{
    // http://msdn.microsoft.com/en-us/data/dn314429.aspx
    public static class FakeDbAsyncExtensions
    {
        public static Mock<DbSet<TEntity>> SetupDataAsync<TEntity>(this Mock<DbSet<TEntity>> dbSet, IQueryable<TEntity> data) where TEntity : class
        {
            dbSet.As<IDbAsyncEnumerable<TEntity>>()
                .Setup(x => x.GetAsyncEnumerator())
                .Returns(new FakeDbAsyncEnumerator<TEntity>(data.GetEnumerator()));
            dbSet.As<IQueryable<TEntity>>()
                .Setup(x => x.Provider)
                .Returns(new FakeDbAsyncQueryProvider<TEntity>(data.Provider));
            dbSet.As<IQueryable<TEntity>>()
                .Setup(x => x.Expression)
                .Returns(data.Expression);
            dbSet.As<IQueryable<TEntity>>()
                .Setup(x => x.ElementType)
                .Returns(data.ElementType);
            dbSet.As<IQueryable<TEntity>>()
                .Setup(x => x.GetEnumerator())
                .Returns(data.GetEnumerator);
            return dbSet;
        }
    }
}