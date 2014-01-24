using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public static class QueryEmailConfirmations
    {
        #region ByTicket

        public static EmailConfirmation ByTicket(this IQueryable<EmailConfirmation> set, string ticket, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefault(ByTicket(ticket)) : set.Single(ByTicket(ticket));
        }

        [UsedImplicitly]
        public static EmailConfirmation ByTicket(this IEnumerable<EmailConfirmation> set, string ticket, bool allowNull = true)
        {
            return set.AsQueryable().ByTicket(ticket, allowNull);
        }

        public static Task<EmailConfirmation> ByTicketAsync(this IQueryable<EmailConfirmation> set, string ticket, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByTicket(ticket)) : set.SingleAsync(ByTicket(ticket));
        }

        [UsedImplicitly]
        public static Task<EmailConfirmation> ByTicketAsync(this IEnumerable<EmailConfirmation> set, string ticket, bool allowNull = true)
        {
            return set.AsQueryable().ByTicketAsync(ticket, allowNull);
        }

        private static Expression<Func<EmailConfirmation, bool>> ByTicket(string ticket)
        {
            return x => x.Ticket.Equals(ticket, StringComparison.Ordinal);
        }

        #endregion
        #region ByOwnerValue

        public static IQueryable<EmailConfirmation> ByOwnerValue(this IQueryable<EmailConfirmation> set, string ownerValue)
        {
            return set.Where(ByOwnerValue(ownerValue));
        }

        [UsedImplicitly]
        public static IEnumerable<EmailConfirmation> ByOwnerValue(this IEnumerable<EmailConfirmation> set, string ownerValue)
        {
            return set.AsQueryable().ByOwnerValue(ownerValue);
        }

        private static Expression<Func<EmailConfirmation, bool>> ByOwnerValue(string ownerValue)
        {
            return x => x.Owner.Value.Equals(ownerValue, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
