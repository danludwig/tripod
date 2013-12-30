using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public static class QueryRemoteMemberships
    {
        #region ByUserId

        public static IQueryable<RemoteMembership> ByUserId(this IQueryable<RemoteMembership> set, int userId)
        {
            return set.Where(ByUserId(userId));
        }

        public static IEnumerable<RemoteMembership> ByUserId(this IEnumerable<RemoteMembership> set, int userId)
        {
            return set.AsQueryable().ByUserId(userId);
        }

        internal static Expression<Func<RemoteMembership, bool>> ByUserId(int userId)
        {
            return x => x.Owner.Id == userId;
        }

        #endregion
        #region ByUserName

        public static IQueryable<RemoteMembership> ByUserName(this IQueryable<RemoteMembership> set, string userName)
        {
            return set.Where(ByUserName(userName));
        }

        public static IEnumerable<RemoteMembership> ByUserName(this IEnumerable<RemoteMembership> set, string userName)
        {
            return set.AsQueryable().ByUserName(userName);
        }

        internal static Expression<Func<RemoteMembership, bool>> ByUserName(string userName)
        {
            return x => x.Owner.Name.Equals(userName, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
        #region ByUserLoginInfo

        public static RemoteMembership ByUserLoginInfo(this IQueryable<RemoteMembership> set,
            UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return allowNull
                ? set.SingleOrDefault(ByUserLoginInfo(userLoginInfo))
                : set.Single(ByUserLoginInfo(userLoginInfo));
        }

        public static RemoteMembership ByUserLoginInfo(this IEnumerable<RemoteMembership> set,
            UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return set.AsQueryable().ByUserLoginInfo(userLoginInfo, allowNull);
        }

        public static Task<RemoteMembership> ByUserLoginInfoAsync(this IQueryable<RemoteMembership> set,
            UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return allowNull
                ? set.SingleOrDefaultAsync(ByUserLoginInfo(userLoginInfo))
                : set.SingleAsync(ByUserLoginInfo(userLoginInfo));
        }

        public static Task<RemoteMembership> ByUserLoginInfoAsync(this IEnumerable<RemoteMembership> set,
            UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return set.AsQueryable().ByUserLoginInfoAsync(userLoginInfo, allowNull);
        }

        internal static Expression<Func<RemoteMembership, bool>> ByUserLoginInfo(UserLoginInfo userLoginInfo)
        {
            return x => x.LoginProvider == userLoginInfo.LoginProvider
                && x.ProviderKey == userLoginInfo.ProviderKey;
        }

        #endregion
        #region ByUserIdAndLoginInfo

        public static RemoteMembership ByUserIdAndLoginInfo(this IQueryable<RemoteMembership> set,
            int userId, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return allowNull
                ? set.SingleOrDefault(ByUserIdAndLoginInfo(userId, userLoginInfo))
                : set.Single(ByUserIdAndLoginInfo(userId, userLoginInfo));
        }

        public static RemoteMembership ByUserIdAndLoginInfo(this IEnumerable<RemoteMembership> set,
            int userId, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return set.AsQueryable().ByUserIdAndLoginInfo(userId, userLoginInfo, allowNull);
        }

        public static Task<RemoteMembership> ByUserIdAndLoginInfoAsync(this IQueryable<RemoteMembership> set,
            int userId, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return allowNull
                ? set.SingleOrDefaultAsync(ByUserIdAndLoginInfo(userId, userLoginInfo))
                : set.SingleAsync(ByUserIdAndLoginInfo(userId, userLoginInfo));
        }

        public static Task<RemoteMembership> ByUserIdAndLoginInfoAsync(this IEnumerable<RemoteMembership> set,
            int userId, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return set.AsQueryable().ByUserIdAndLoginInfoAsync(userId, userLoginInfo, allowNull);
        }

        internal static Expression<Func<RemoteMembership, bool>> ByUserIdAndLoginInfo(int userId, UserLoginInfo userLoginInfo)
        {
            return x => x.Owner.Id == userId
                && x.LoginProvider == userLoginInfo.LoginProvider
                && x.ProviderKey == userLoginInfo.ProviderKey;
        }

        #endregion
        #region ByUserNameAndLoginInfo

        public static RemoteMembership ByUserNameAndLoginInfo(this IQueryable<RemoteMembership> set,
            string userName, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return allowNull
                ? set.SingleOrDefault(ByUserNameAndLoginInfo(userName, userLoginInfo))
                : set.Single(ByUserNameAndLoginInfo(userName, userLoginInfo));
        }

        public static RemoteMembership ByUserNameAndLoginInfo(this IEnumerable<RemoteMembership> set,
            string userName, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return set.AsQueryable().ByUserNameAndLoginInfo(userName, userLoginInfo, allowNull);
        }

        public static Task<RemoteMembership> ByUserNameAndLoginInfoAsync(this IQueryable<RemoteMembership> set,
            string userName, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return allowNull
                ? set.SingleOrDefaultAsync(ByUserNameAndLoginInfo(userName, userLoginInfo))
                : set.SingleAsync(ByUserNameAndLoginInfo(userName, userLoginInfo));
        }

        public static Task<RemoteMembership> ByUserNameAndLoginInfoAsync(this IEnumerable<RemoteMembership> set,
            string userName, UserLoginInfo userLoginInfo, bool allowNull = true)
        {
            return set.AsQueryable().ByUserNameAndLoginInfoAsync(userName, userLoginInfo, allowNull);
        }

        internal static Expression<Func<RemoteMembership, bool>> ByUserNameAndLoginInfo(string userName, UserLoginInfo userLoginInfo)
        {
            return x => x.Owner.Name.Equals(userName, StringComparison.OrdinalIgnoreCase)
                && x.LoginProvider == userLoginInfo.LoginProvider
                && x.ProviderKey == userLoginInfo.ProviderKey;
        }

        #endregion
    }
}
