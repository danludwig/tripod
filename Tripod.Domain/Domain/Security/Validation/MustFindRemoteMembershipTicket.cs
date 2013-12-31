using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustFindRemoteMembershipTicket : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindRemoteMembershipTicket(IProcessQueries queries)
            : base(() => "There is no external login information to process.")
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            if (principal == null) return false;

            var query =  new GetRemoteMembershipTicket(principal);
            var ticket = _queries.Execute(query).Result;
            return ticket != null;
        }
    }

    public static class MustFindRemoteMembershipTicketExtensions
    {
        public static IRuleBuilderOptions<T, IPrincipal> MustFindRemoteMembershipTicket<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindRemoteMembershipTicket(queries));
        }
    }
}
