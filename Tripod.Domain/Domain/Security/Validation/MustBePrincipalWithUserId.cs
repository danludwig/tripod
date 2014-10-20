using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public static class MustBePrincipalWithUserIdExtensions
    {
        /// <summary>
        /// Validates that this IPrincipal has an expected User Id value.
        /// </summary>
        /// <typeparam name="T">The command with the EmailAddress id to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="userId">The expected UserId property value of this IPrincipal.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, IPrincipal> MustBePrincipalWithUserId<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, Func<T, int> userId)
        {
            return ruleBuilder.SetValidator(new MustBePrincipalWithUserId<T>(userId));
        }
    }

    public class MustBePrincipalWithUserId<T> : PropertyValidator
    {
        private readonly Func<T, int> _userId;

        internal MustBePrincipalWithUserId(Func<T, int> userId)
            : base(() => Resources.Validation_NotAuthorized_UserAction)
        {
            if (userId == null) throw new ArgumentNullException("userId");
            _userId = userId;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            var userId = _userId((T)context.Instance);
            if (principal.Identity.GetUserId<int>() == userId) return true;

            context.MessageFormatter.AppendArgument("PropertyValue", principal.Identity.Name);
            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            context.MessageFormatter.AppendArgument("UserId", userId);
            return false;
        }
    }
}
