using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeUnauthenticatedPrincipal : PropertyValidator
    {
        internal MustBeUnauthenticatedPrincipal()
            : base(() => Resources.Validation_Principal_MustBeUnauthenticated)
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            return principal == null || !principal.Identity.IsAuthenticated;
        }
    }

    public static class MustBeUnauthenticatedPrincipalExtensions
    {
        /// <summary>
        /// Validates that this IPrincipal is either null or has an unauthenticated identity.
        /// </summary>
        /// <typeparam name="T">The command with the Principal to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, IPrincipal> MustBeUnauthenticatedPrincipal<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MustBeUnauthenticatedPrincipal());
        }
    }
}
