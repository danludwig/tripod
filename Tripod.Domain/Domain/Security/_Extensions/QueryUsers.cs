using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public static class QueryUsers
    {
        #region ByName

        public static User ByName(this IQueryable<User> set, string name, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByName(name)) : set.Single(ByName(name));
        }

        public static User ByName(this IEnumerable<User> set, string name, bool allowNull = true)
        {
            return set.AsQueryable().ByName(name, allowNull);
        }

        public static Task<User> ByNameAsync(this IQueryable<User> set, string name, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByName(name)) : set.SingleAsync(ByName(name));
        }

        public static Task<User> ByNameAsync(this IEnumerable<User> set, string name, bool allowNull = true)
        {
            return set.AsQueryable().ByNameAsync(name, allowNull);
        }

        private static Expression<Func<User, bool>> ByName(string name)
        {
            return x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
