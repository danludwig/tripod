using FluentValidation;

namespace Tripod
{
    public class FakeStringMaxLengthValidator : AbstractValidator<FakeStringLengthCommand>
    {
        public FakeStringMaxLengthValidator(int maxLength)
        {
            RuleFor(x => x.StringProperty).MaxLength(maxLength);
        }
    }
}