using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public static class QueryEmailAddresses
    {
        #region ByValue

        public static EmailAddress ByValue(this IQueryable<EmailAddress> set, string value, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByValue(value)) : set.Single(ByValue(value));
        }

        public static EmailAddress ByValue(this IEnumerable<EmailAddress> set, string value, bool allowNull = true)
        {
            return set.AsQueryable().ByValue(value, allowNull);
        }

        public static Task<EmailAddress> ByValueAsync(this IQueryable<EmailAddress> set, string value, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByValue(value)) : set.SingleAsync(ByValue(value));
        }

        public static Task<EmailAddress> ByValueAsync(this IEnumerable<EmailAddress> set, string value, bool allowNull = true)
        {
            return set.AsQueryable().ByValueAsync(value, allowNull);
        }

        private static Expression<Func<EmailAddress, bool>> ByValue(string value)
        {
            return x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
        #region ByUserId

        public static IQueryable<EmailAddress> ByUserId(this IQueryable<EmailAddress> set, int userId)
        {
            return set.Where(ByUserId(userId));
        }

        public static IEnumerable<EmailAddress> ByUserId(this IEnumerable<EmailAddress> set, int userId)
        {
            return set.AsQueryable().ByUserId(userId);
        }

        internal static Expression<Func<EmailAddress, bool>> ByUserId(int userId)
        {
            return x => x.UserId == userId;
        }

        #endregion
    }
}
