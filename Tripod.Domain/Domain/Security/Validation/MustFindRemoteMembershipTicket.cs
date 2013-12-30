using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustFindRemoteMembershipTicket<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, string> _xsrfKey;

        internal MustFindRemoteMembershipTicket(IProcessQueries queries, Func<T, string> xsrfKey)
            : base(() => "There is no external login information to process.")
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
            _xsrfKey = xsrfKey;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            var xsrfKey = _xsrfKey != null ? _xsrfKey((T)context.Instance) : null;
            if (principal == null) return false;

            var query = principal.Identity.IsAuthenticated
                ? new GetRemoteMembershipTicket(principal, xsrfKey)
                : new GetRemoteMembershipTicket();
            var ticket = _queries.Execute(query).Result;
            return ticket != null;
        }
    }

    public static class MustFindRemoteMembershipTicketExtensions
    {
        public static IRuleBuilderOptions<T, IPrincipal> MustFindRemoteMembershipTicket<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries, Func<T, string> xsrfKey = null)
        {
            return ruleBuilder.SetValidator(new MustFindRemoteMembershipTicket<T>(queries, xsrfKey));
        }
    }
}
