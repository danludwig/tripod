using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeMustBeValidUserNameCommand
    {
        public string UserName { get; set; }
    }

    public class FakeMustBeValidUserNameValidator : AbstractValidator<FakeMustBeValidUserNameCommand>
    {
        public FakeMustBeValidUserNameValidator()
        {
            RuleFor(x => x.UserName)
                .MustBeValidUserName()
                .WithName(User.Constraints.NameLabel)
            ;
        }
    }
}