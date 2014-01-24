using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public static class QueryLocalMemberships
    {
        #region ByUserId

        [UsedImplicitly]
        public static LocalMembership ByUserId(this IQueryable<LocalMembership> set, int userId, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByUserId(userId)) : set.Single(ByUserId(userId));
        }

        [UsedImplicitly]
        public static LocalMembership ByUserId(this IEnumerable<LocalMembership> set, int userId, bool allowNull = true)
        {
            return set.AsQueryable().ByUserId(userId, allowNull);
        }

        public static Task<LocalMembership> ByUserIdAsync(this IQueryable<LocalMembership> set, int userId, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByUserId(userId)) : set.SingleAsync(ByUserId(userId));
        }

        [UsedImplicitly]
        public static Task<LocalMembership> ByUserIdAsync(this IEnumerable<LocalMembership> set, int userId, bool allowNull = true)
        {
            return set.AsQueryable().ByUserIdAsync(userId, allowNull);
        }

        internal static Expression<Func<LocalMembership, bool>> ByUserId(int userId)
        {
            return x => x.Owner.Id == userId;
        }

        #endregion
        #region ByUserName

        [UsedImplicitly]
        public static LocalMembership ByUserName(this IQueryable<LocalMembership> set, string userName, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByUserName(userName)) : set.Single(ByUserName(userName));
        }

        [UsedImplicitly]
        public static LocalMembership ByUserName(this IEnumerable<LocalMembership> set, string userName, bool allowNull = true)
        {
            return set.AsQueryable().ByUserName(userName, allowNull);
        }

        public static Task<LocalMembership> ByUserNameAsync(this IQueryable<LocalMembership> set, string userName, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByUserName(userName)) : set.SingleAsync(ByUserName(userName));
        }

        [UsedImplicitly]
        public static Task<LocalMembership> ByUserNameAsync(this IEnumerable<LocalMembership> set, string userName, bool allowNull = true)
        {
            return set.AsQueryable().ByUserNameAsync(userName, allowNull);
        }

        internal static Expression<Func<LocalMembership, bool>> ByUserName(string userName)
        {
            return x => x.Owner.Name.Equals(userName, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
        #region ByUserLoginInfo

        [UsedImplicitly]
        public static LocalMembership ByUserLoginInfo(this IQueryable<LocalMembership> set, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByUserLoginInfo(userLoginInfo)) : set.Single(ByUserLoginInfo(userLoginInfo));
        }

        [UsedImplicitly]
        public static LocalMembership ByUserLoginInfo(this IEnumerable<LocalMembership> set, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return set.AsQueryable().ByUserLoginInfo(userLoginInfo, allowNull);
        }

        public static Task<LocalMembership> ByUserLoginInfoAsync(this IQueryable<LocalMembership> set, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByUserLoginInfo(userLoginInfo)) : set.SingleAsync(ByUserLoginInfo(userLoginInfo));
        }

        [UsedImplicitly]
        public static Task<LocalMembership> ByUserLoginInfoAsync(this IEnumerable<LocalMembership> set, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return set.AsQueryable().ByUserLoginInfoAsync(userLoginInfo, allowNull);
        }

        private static Expression<Func<LocalMembership, bool>> ByUserLoginInfo(UserLoginInfo userLoginInfo)
        {
            return x => x.Owner.RemoteMemberships.Any(y => y.LoginProvider == userLoginInfo.LoginProvider && y.ProviderKey == userLoginInfo.ProviderKey);
        }

        #endregion
    }
}
