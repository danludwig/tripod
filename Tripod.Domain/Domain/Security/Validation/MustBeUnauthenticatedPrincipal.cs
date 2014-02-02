using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeUnauthenticatedPrincipal : PropertyValidator
    {
        internal MustBeUnauthenticatedPrincipal()
            : base(() => "You must sign out in order to perform this action.")
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
        public static IRuleBuilderOptions<T, IPrincipal> MustBeUnauthenticatedPrincipal<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MustBeUnauthenticatedPrincipal());
        }
    }
}
