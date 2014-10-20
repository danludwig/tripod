using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeMustNotContainInvalidUserNameTextCommand
    {
        public string UserName { get; set; }
    }

    public class FakeMustNotContainInvalidUserNameTextValidator : AbstractValidator<FakeMustNotContainInvalidUserNameTextCommand>
    {
        public FakeMustNotContainInvalidUserNameTextValidator()
        {
            RuleFor(x => x.UserName)
                .MustNotContainInvalidUserNameText()
                .WithName(User.Constraints.NameLabel)
            ;
        }
    }
}