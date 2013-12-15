using FluentValidation;

namespace Tripod
{
    public class FakeStringMinLengthValidator : AbstractValidator<FakeStringLengthCommand>
    {
        public FakeStringMinLengthValidator(int minLength)
        {
            RuleFor(x => x.StringProperty).MinLength(minLength);
        }
    }
}