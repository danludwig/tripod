using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeVerifiableEmailAddressExtensions
    {
        /// <summary>
        /// Validates that this EmailAddress is not empty, is in email address format,
        /// does not exceed maximum length requirements, and is not already verified.
        /// </summary>
        /// <typeparam name="T">The command with the EmailAddress Value to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating EmailAddress by Value.</param>
        /// <returns>Fluent rule builder options.</returns>
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
