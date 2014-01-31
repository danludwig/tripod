using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeVerifiableEmailAddressExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeVerifiableEmailAddress<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder
                .NotEmpty()
                .EmailAddress()
                .MaxLength(EmailAddress.Constraints.ValueMaxLength)
                .MustNotBeVerifiedEmailAddress(queries)
            ;
        }
    }
}
