using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tripod.Web
{
    public static class SessionExtensions
    {
        private const string ConfirmEmailTicketsKey = "EmailConfirmationTickets";

        public static void AddConfirmEmailTicket(this HttpSessionStateBase session, string ticket)
        {
            var tickets = session.ConfirmEmailTickets().ToList();
            tickets.Add(ticket);
            session[ConfirmEmailTicketsKey] = tickets.ToArray();
        }

        public static IEnumerable<string> ConfirmEmailTickets(this HttpSessionStateBase session)
        {
            var tickets = session[ConfirmEmailTicketsKey] as string[];
            if (tickets != null) return tickets;
            tickets = new string[0];
            session[ConfirmEmailTicketsKey] = tickets;
            return tickets;
        }
    }
}