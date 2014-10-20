using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustNotContainInvalidUserNameTextExtensions
    {
        /// <summary>
        /// Validates that this User Name contains only valid text characters.
        /// </summary>
        /// <typeparam name="T">The command with the User Name to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustNotContainInvalidUserNameText<T>
            (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MustNotContainInvalidUserNameText());
        }
    }

    internal class MustNotContainInvalidUserNameText : PropertyValidator
    {
        internal MustNotContainInvalidUserNameText()
            : base(() => Resources.Validation_UserName_AllowedCharacters)
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var userName = (string)context.PropertyValue;

            // all email addresses are automatically valid user names
            if (userName == null || EmailAddress.ValueRegex.IsMatch(userName)) return true;

            // otherwise, must not have invalid characters.
            return userName.All(x => User.Constraints.AllowedNameCharacters.Contains(x));
        }
    }
}
