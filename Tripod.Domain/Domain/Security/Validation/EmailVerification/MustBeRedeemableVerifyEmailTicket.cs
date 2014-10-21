using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeRedeemableVerifyEmailTicketExtensions
    {
        /// <summary>
        /// Validates that an EmailVerification Ticket is not empty, exists in the data store,
        /// is not already redeemed, and is not expired.
        /// </summary>
        /// <typeparam name="T">The command with the EmailVerification Ticket to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance.</param>
        /// <returns>Fluent rule builder options.</returns>
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
