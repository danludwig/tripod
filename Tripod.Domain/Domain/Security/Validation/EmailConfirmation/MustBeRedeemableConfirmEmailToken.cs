using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeRedeemableConfirmEmailTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeRedeemableConfirmEmailToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder
                .NotEmpty()
                .MustBeValidConfirmEmailToken(queries)
                .MustFindEmailConfirmationByToken(queries)
                .MustNotBeRedeemedConfirmEmailToken(queries)
                .MustNotBeExpiredConfirmEmailToken(queries)
            ;
        }
    }
}
