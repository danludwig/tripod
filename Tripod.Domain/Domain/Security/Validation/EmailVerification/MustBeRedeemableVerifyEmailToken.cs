using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeRedeemableVerifyEmailTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeRedeemableVerifyEmailToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder
                .NotEmpty()
                .MustBeValidVerifyEmailToken(queries)
                .MustFindEmailVerificationByToken(queries)
                .MustNotBeRedeemedVerifyEmailToken(queries)
                .MustNotBeExpiredVerifyEmailToken(queries)
            ;
        }
    }
}
