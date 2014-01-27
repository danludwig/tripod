using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotContainInvalidUserNameText : PropertyValidator
    {
        private const string ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";

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
            return userName.All(x => ValidCharacters.Contains(x));
        }
    }

    public static class MustNotContainInvalidUserNameTextExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotContainInvalidUserNameText<T>
            (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MustNotContainInvalidUserNameText());
        }
    }
}
