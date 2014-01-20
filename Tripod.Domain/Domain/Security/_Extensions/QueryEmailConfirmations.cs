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

        public static EmailConfirmation ByTicket(this IEnumerable<EmailConfirmation> set, string ticket, bool allowNull = true)
        {
            return set.AsQueryable().ByTicket(ticket, allowNull);
        }

        public static Task<EmailConfirmation> ByTicketAsync(this IQueryable<EmailConfirmation> set, string ticket, bool allowNull = true)
        {
            return allowNull ? set.SingleOrDefaultAsync(ByTicket(ticket)) : set.SingleAsync(ByTicket(ticket));
        }

        public static Task<EmailConfirmation> ByTicketAsync(this IEnumerable<EmailConfirmation> set, string ticket, bool allowNull = true)
        {
            return set.AsQueryable().ByTicketAsync(ticket, allowNull);
        }

        private static Expression<Func<EmailConfirmation, bool>> ByTicket(string ticket)
        {
            return x => x.Ticket == ticket;
        }

        #endregion
    }
}
