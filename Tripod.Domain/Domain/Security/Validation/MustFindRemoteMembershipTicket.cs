using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustFindRemoteMembershipTicketExtensions
    {
        /// <summary>
        /// Validates that this principal has a just authenticated remotely with a 3rd party login provider.
        /// </summary>
        /// <typeparam name="T">The command with the Principal to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for checking the remote membership authentication.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, IPrincipal> MustFindRemoteMembershipTicket<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindRemoteMembershipTicket(queries));
        }
    }

    internal class MustFindRemoteMembershipTicket : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindRemoteMembershipTicket(IProcessQueries queries)
            : base(() => Resources.Validation_RemoteMembership_NoTicket)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            if (principal == null) return false;

            var query =  new PrincipalRemoteMembershipTicket(principal);
            var ticket = _queries.Execute(query).Result;
            return ticket != null;
        }
    }
}
