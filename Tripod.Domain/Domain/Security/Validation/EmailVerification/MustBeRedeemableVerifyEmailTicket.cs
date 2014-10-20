using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeRedeemableVerifyEmailTicketExtensions
    {
        /// <summary>
        /// Validates that an email verification ticket is not empty, exists in the data store, is not already redeemed,
        /// and is not expired.
        /// </summary>
        /// <typeparam name="T">The command or other data transfer object to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent IRuleBuilderOptions for method chaining.</param>
        /// <param name="queries">Query processor instance.</param>
        /// <returns>Fluent IRuleBuilderOptions for method chaining.</returns>
        public static IRuleBuilderOptions<T, string> MustBeRedeemableVerifyEmailTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder
                .NotEmpty()
                .MustFindEmailVerificationByTicket(queries)
                .MustNotBeRedeemedVerifyEmailTicket(queries)
                .MustNotBeExpiredVerifyEmailTicket(queries)
            ;
        }
    }
}
