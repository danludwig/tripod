using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeValidPasswordExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValidPassword<T>
            (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .MinLength(LocalMembership.Constraints.PasswordMinLength)
                .MaxLength(LocalMembership.Constraints.PasswordMaxLength)
            ;
        }
    }
}