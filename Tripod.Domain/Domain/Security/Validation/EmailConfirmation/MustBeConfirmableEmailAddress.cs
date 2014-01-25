using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeConfirmableEmailAddressExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeConfirmableEmailAddress<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder
                .NotEmpty()
                .EmailAddress()
                .MaxLength(EmailAddress.Constraints.ValueMaxLength)
                .MustNotBeConfirmedEmailAddress(queries)
            ;
        }
    }
}
