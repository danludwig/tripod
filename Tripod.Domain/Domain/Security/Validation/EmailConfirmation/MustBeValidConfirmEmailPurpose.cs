using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeValidConfirmEmailPurpose : PropertyValidator
    {
        internal MustBeValidConfirmEmailPurpose()
            : base(() => Resources.Validation_EmailConfirmationPurpose_IsEmpty)
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var purpose = (EmailConfirmationPurpose)context.PropertyValue;
            if (purpose != EmailConfirmationPurpose.Invalid) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustBeValidConfirmEmailPurposeExtensions
    {
        public static IRuleBuilderOptions<T, EmailConfirmationPurpose> MustBeValidConfirmEmailPurpose<T>
            (this IRuleBuilder<T, EmailConfirmationPurpose> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MustBeValidConfirmEmailPurpose());
        }
    }
}
