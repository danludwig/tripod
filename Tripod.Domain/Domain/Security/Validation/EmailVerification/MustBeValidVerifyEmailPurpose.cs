using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustBeValidVerifyEmailPurposeExtensions
    {
        public static IRuleBuilderOptions<T, EmailVerificationPurpose> MustBeValidVerifyEmailPurpose<T>
            (this IRuleBuilder<T, EmailVerificationPurpose> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MustBeValidVerifyEmailPurpose());
        }
    }

    internal class MustBeValidVerifyEmailPurpose : PropertyValidator
    {
        internal MustBeValidVerifyEmailPurpose()
            : base(() => Resources.Validation_EmailVerificationPurpose_IsEmpty)
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var purpose = (EmailVerificationPurpose)context.PropertyValue;
            if (purpose != EmailVerificationPurpose.Invalid) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }
}
