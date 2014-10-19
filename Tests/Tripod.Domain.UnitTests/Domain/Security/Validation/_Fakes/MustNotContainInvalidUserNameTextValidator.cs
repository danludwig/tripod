using FluentValidation;

namespace Tripod.Domain.Security
{
    public class MustNotContainInvalidUserNameTextCommand
    {
        public string UserName { get; set; }
    }

    public class MustNotContainInvalidUserNameTextValidator : AbstractValidator<MustNotContainInvalidUserNameTextCommand>
    {
        public MustNotContainInvalidUserNameTextValidator()
        {
            RuleFor(x => x.UserName)
                .MustNotContainInvalidUserNameText()
                .WithName(User.Constraints.NameLabel)
            ;
        }
    }
}