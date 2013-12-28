using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeValidUserNameExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValidUserName<T>
            (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .MinLength(User.Constraints.NameMinLength)
                .MaxLength(User.Constraints.NameMaxLength)
            ;
        }
    }
}
