using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeValidPasswordExtensions
    {
        /// <summary>
        /// Validates that a password is not empty, and meets minimum & maximum length range requirements.
        /// </summary>
        /// <typeparam name="T">The command with the password to be validated.</typeparam>
        /// <param name="ruleBuilder">Rule builder options for fluent interface.</param>
        /// <returns>Rule builder options for fluent interface.</returns>
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