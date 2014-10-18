using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod
{
    public class MinLength : PropertyValidator
    {
        private readonly int _minLength;

        internal MinLength(int minLength) : base(() => Resources.Validation_MinLength)
        {
            if (minLength < 1) throw new ArgumentOutOfRangeException("minLength",
                string.Format(Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
            _minLength = minLength;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var text = (string)context.PropertyValue;

            // assert that text satisfies character limit
            if (text == null || text.Length >= _minLength) return true;

            context.MessageFormatter.AppendArgument("MinLength", _minLength);
            context.MessageFormatter.AppendArgument("TotalLength", text.Length);
            context.MessageFormatter.AppendArgument("Characters", text.Length == 1
                ? Resources.Validation_CharacterLower : Resources.Validation_CharactersLower);
            return false;
        }
    }

    public static class MinLengthExtensions
    {
        /// <summary>
        /// Validates that this string property's length is not less than a specified value.
        /// </summary>
        /// <typeparam name="T">The command with a string property to validate minimum length of.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="minLength">Minimum length allowed for this string property.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MinLength<T>
            (this IRuleBuilder<T, string> ruleBuilder, int minLength)
        {
            return ruleBuilder.SetValidator(new MinLength(minLength));
        }
    }
}
