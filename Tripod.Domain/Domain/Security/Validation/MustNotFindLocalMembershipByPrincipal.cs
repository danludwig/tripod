using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public static class MustNotFindLocalMembershipByPrincipalExtensions
    {
        /// <summary>
        /// Validates that this Principal does not have a LocalMembership.
        /// </summary>
        /// <typeparam name="T">The command with the Principal to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating LocalMembership by Principal.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, IPrincipal> MustNotFindLocalMembershipByPrincipal<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotFindLocalMembershipByPrincipal(queries));
        }
    }

    internal class MustNotFindLocalMembershipByPrincipal : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotFindLocalMembershipByPrincipal(IProcessQueries queries)
            : base(() => Resources.Validation_LocalMembershipByUser_AlreadyExists)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            if (principal == null) return true;

            if (!principal.Identity.HasAppUserId()) return true;
            var entity = _queries.Execute(new LocalMembershipByUser(principal.Identity.GetUserId<int>())).Result;

            if (entity == null) return true;

            context.MessageFormatter.AppendArgument("PropertyValue", principal.Identity.Name);
            context.MessageFormatter.AppendArgument("PasswordLabel", LocalMembership.Constraints.Label.ToLower());
            return false;
        }
    }
}
