using FluentValidation;

namespace Tripod.Domain.Security
{
    public static class MustBeValidUserNameExtensions
    {
        /// <summary>
        /// Validates that this user name is not empty, meets minimum & maximum string length arguments,
        /// and does not contain any invalid username characters.
        /// </summary>
        /// <typeparam name="T">The command with the User Name to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustBeValidUserName<T>
            (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .MinLength(User.Constraints.NameMinLength)
                .MaxLength(User.Constraints.NameMaxLength)
                .MustNotContainInvalidUserNameText()
            ;
        }
    }
}
