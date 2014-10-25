using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

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

        internal static Expression<Func<User, bool>> ByName(string name)
        {
            return x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
        #region ByUserLoginInfo

        public static User ByUserLoginInfo(this IQueryable<User> set, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByUserLoginInfo(userLoginInfo)) : set.Single(ByUserLoginInfo(userLoginInfo));
        }

        public static User ByUserLoginInfo(this IEnumerable<User> set, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return set.AsQueryable().ByUserLoginInfo(userLoginInfo, allowNull);
        }

        public static Task<User> ByUserLoginInfoAsync(this IQueryable<User> set, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByUserLoginInfo(userLoginInfo)) : set.SingleAsync(ByUserLoginInfo(userLoginInfo));
        }

        public static Task<User> ByUserLoginInfoAsync(this IEnumerable<User> set, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return set.AsQueryable().ByUserLoginInfoAsync(userLoginInfo, allowNull);
        }

        private static Expression<Func<User, bool>> ByUserLoginInfo(UserLoginInfo userLoginInfo)
        {
            return x => x.RemoteMemberships.Any(y => y.LoginProvider == userLoginInfo.LoginProvider && y.ProviderKey == userLoginInfo.ProviderKey);
        }

        #endregion
    }
}
