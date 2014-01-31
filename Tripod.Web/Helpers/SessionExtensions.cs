using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tripod.Web
{
    public static class SessionExtensions
    {
        private const string VerifyEmailTicketsKey = "EmailVerificationTickets";

        public static void VerifyEmailTickets(this HttpSessionStateBase session, string ticket)
        {
            var tickets = session.VerifyEmailTickets().ToList();
            if (!string.IsNullOrWhiteSpace(ticket)) tickets.Add(ticket);
            else tickets.Clear();
            session[VerifyEmailTicketsKey] = tickets.ToArray();
        }

        public static IEnumerable<string> VerifyEmailTickets(this HttpSessionStateBase session)
        {
            var tickets = session[VerifyEmailTicketsKey] as string[];
            if (tickets != null) return tickets;
            tickets = new string[0];
            session[VerifyEmailTicketsKey] = tickets;
            return tickets;
        }
    }
}