using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod
{
    public class MaxLength : PropertyValidator
    {
        private readonly int _maxLength;

        internal MaxLength(int maxLength)
            : base(() => Resources.Validation_MaxLength)
        {
            if (maxLength < 1) throw new ArgumentOutOfRangeException("maxLength",
                Resources.Exception_ArgumentOutOfRange_CannotBeLessThanOne);
            _maxLength = maxLength;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var text = (string)context.PropertyValue;

            // assert that text does not exceed character limit
            if (text == null || text.Length <= _maxLength) return true;

            context.MessageFormatter.AppendArgument("MaxLength", _maxLength);
            context.MessageFormatter.AppendArgument("TotalLength", text.Length);
            return false;
        }
    }

    public static class MaxLengthExtensions
    {
        public static IRuleBuilderOptions<T, string> MaxLength<T>
            (this IRuleBuilder<T, string> ruleBuilder, int maxLength)
        {
            return ruleBuilder.SetValidator(new MaxLength(maxLength));
        }
    }
}
