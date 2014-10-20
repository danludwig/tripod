using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public static class MustFindLocalMembershipByPrincipalExtensions
    {
        /// <summary>
        /// Validates that a LocalMembership exists in the underlying data store for this Principal.
        /// </summary>
        /// <typeparam name="T">The command with the Principal to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating LocalMembership by Principal.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, IPrincipal> MustFindLocalMembershipByPrincipal<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindLocalMembershipByPrincipal(queries));
        }
    }

    internal class MustFindLocalMembershipByPrincipal : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindLocalMembershipByPrincipal(IProcessQueries queries)
            : base(() => Resources.Validation_LocalMembershipByUser_DoesNotExist)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            if (principal != null)
            {
                if (principal.Identity.HasAppUserId())
                {
                    var userId = principal.Identity.GetUserId<int>();
                    var entity = _queries.Execute(new LocalMembershipByUser(userId)).Result;
                    if (entity != null) return true;
                }
            }

            context.MessageFormatter.AppendArgument("PropertyValue",principal != null ? principal.Identity.Name : "");
            context.MessageFormatter.AppendArgument("PasswordLabel", LocalMembership.Constraints.Label.ToLower());
            return false;
        }
    }
}
