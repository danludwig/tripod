using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public static class QueryEmailVerifications
    {
        #region ByTicket

        public static EmailVerification ByTicket(this IQueryable<EmailVerification> set, string ticket, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByTicket(ticket)) : set.Single(ByTicket(ticket));
        }

        [UsedImplicitly]
        public static EmailVerification ByTicket(this IEnumerable<EmailVerification> set, string ticket, bool allowNull = true)
        {
            return set.AsQueryable().ByTicket(ticket, allowNull);
        }

        public static Task<EmailVerification> ByTicketAsync(this IQueryable<EmailVerification> set, string ticket, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByTicket(ticket)) : set.SingleAsync(ByTicket(ticket));
        }

        [UsedImplicitly]
        public static Task<EmailVerification> ByTicketAsync(this IEnumerable<EmailVerification> set, string ticket, bool allowNull = true)
        {
            return set.AsQueryable().ByTicketAsync(ticket, allowNull);
        }

        private static Expression<Func<EmailVerification, bool>> ByTicket(string ticket)
        {
            return x => x.Ticket.Equals(ticket, StringComparison.Ordinal);
        }

        #endregion
        #region ByOwnerId

        public static IQueryable<EmailVerification> ByOwnerId(this IQueryable<EmailVerification> set, int ownerId)
        {
            return set.Where(ByOwnerId(ownerId));
        }

        [UsedImplicitly]
        public static IEnumerable<EmailVerification> ByOwnerId(this IEnumerable<EmailVerification> set, int ownerId)
        {
            return set.AsQueryable().ByOwnerId(ownerId);
        }

        private static Expression<Func<EmailVerification, bool>> ByOwnerId(int ownerId)
        {
            return x => x.OwnerId == ownerId;
        }

        #endregion
    }
}
