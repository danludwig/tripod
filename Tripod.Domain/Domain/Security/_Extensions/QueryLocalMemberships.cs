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

        public static LocalMembership ByUserId(this IQueryable<LocalMembership> set, int userId, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByUserId(userId)) : set.Single(ByUserId(userId));
        }

        public static LocalMembership ByUserId(this IEnumerable<LocalMembership> set, int userId, bool allowNull = true)
        {
            return set.AsQueryable().ByUserId(userId, allowNull);
        }

        public static Task<LocalMembership> ByUserIdAsync(this IQueryable<LocalMembership> set, int userId, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByUserId(userId)) : set.SingleAsync(ByUserId(userId));
        }

        public static Task<LocalMembership> ByUserIdAsync(this IEnumerable<LocalMembership> set, int userId, bool allowNull = true)
        {
            return set.AsQueryable().ByUserIdAsync(userId, allowNull);
        }

        internal static Expression<Func<LocalMembership, bool>> ByUserId(int userId)
        {
            return x => x.User.Id == userId;
        }

        #endregion
        #region ByUserName

        public static LocalMembership ByUserName(this IQueryable<LocalMembership> set, string userName, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByUserName(userName)) : set.Single(ByUserName(userName));
        }

        public static LocalMembership ByUserName(this IEnumerable<LocalMembership> set, string userName, bool allowNull = true)
        {
            return set.AsQueryable().ByUserName(userName, allowNull);
        }

        public static Task<LocalMembership> ByUserNameAsync(this IQueryable<LocalMembership> set, string userName, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByUserName(userName)) : set.SingleAsync(ByUserName(userName));
        }

        public static Task<LocalMembership> ByUserNameAsync(this IEnumerable<LocalMembership> set, string userName, bool allowNull = true)
        {
            return set.AsQueryable().ByUserNameAsync(userName, allowNull);
        }

        internal static Expression<Func<LocalMembership, bool>> ByUserName(string userName)
        {
            return x => x.User.Name.Equals(userName, StringComparison.OrdinalIgnoreCase);
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
            return x => x.User.RemoteMemberships.Any(y => y.LoginProvider == userLoginInfo.LoginProvider && y.ProviderKey == userLoginInfo.ProviderKey);
        }

        #endregion
        #region ByVerifiedEmail

        [UsedImplicitly]
        public static LocalMembership ByVerifiedEmail(this IQueryable<LocalMembership> set, string emailAddress, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByVerifiedEmail(emailAddress)) : set.Single(ByVerifiedEmail(emailAddress));
        }

        [UsedImplicitly]
        public static LocalMembership ByVerifiedEmail(this IEnumerable<LocalMembership> set, string emailAddress, bool allowNull = true)
        {
            return set.AsQueryable().ByVerifiedEmail(emailAddress, allowNull);
        }

        public static Task<LocalMembership> ByVerifiedEmailAsync(this IQueryable<LocalMembership> set, string emailAddress, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByVerifiedEmail(emailAddress)) : set.SingleAsync(ByVerifiedEmail(emailAddress));
        }

        [UsedImplicitly]
        public static Task<LocalMembership> ByVerifiedEmailAsync(this IEnumerable<LocalMembership> set, string emailAddress, bool allowNull = true)
        {
            return set.AsQueryable().ByVerifiedEmailAsync(emailAddress, allowNull);
        }

        private static Expression<Func<LocalMembership, bool>> ByVerifiedEmail(string emailAddress)
        {
            return x => x.User.EmailAddresses.Any(y => y.IsVerified && y.Value.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}
