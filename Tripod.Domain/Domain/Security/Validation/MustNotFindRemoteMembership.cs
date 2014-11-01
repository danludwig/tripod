using System;
using System.Linq.Expressions;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public static class MustNotFindRemoteMembershipExtensions
    {
        /// <summary>
        /// Validates that this principal owns the remote membership ticket.
        /// </summary>
        /// <typeparam name="T">The command with the Principal to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating the remote membership ticket and entity.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, IPrincipal> MustNotFindRemoteMembership<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotFindRemoteMembership(queries));
        }
    }

    internal class MustNotFindRemoteMembership : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotFindRemoteMembership(IProcessQueries queries)
            : base(() => Resources.Validation_RemoteMembership_AlreadyAssigned)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            if (principal == null) return true;

            var remoteMembershipTicketQuery = new PrincipalRemoteMembershipTicket(principal);
            var remoteMembershipTicket = _queries.Execute(remoteMembershipTicketQuery).Result;

            // another validator should verify the presence of the remote membership ticket
            if (remoteMembershipTicket == null) return true;

            var remoteMembershipQuery = new RemoteMembershipBy(remoteMembershipTicket.Login)
            {
                EagerLoad = new Expression<Func<RemoteMembership, object>>[]
                {
                    x => x.User,
                }
            };
            var remoteMembership = _queries.Execute(remoteMembershipQuery).Result;

            // validation passes if the remote membership does not already exist,
            // or exists and is already owned by this principal.
            if (remoteMembership == null ||
                remoteMembership.UserId == principal.Identity.GetUserId<int>())
                return true;

            context.MessageFormatter.AppendArgument("ProviderName", remoteMembershipTicket.Login.LoginProvider);
            return false;
        }
    }
}
