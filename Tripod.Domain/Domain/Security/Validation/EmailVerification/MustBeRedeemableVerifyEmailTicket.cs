using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeRedeemableVerifyEmailicketExtensions
    {
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
