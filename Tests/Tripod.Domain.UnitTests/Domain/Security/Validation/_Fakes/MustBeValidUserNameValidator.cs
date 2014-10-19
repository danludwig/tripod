using FluentValidation;

namespace Tripod.Domain.Security
{
    public class MustBeValidUserNameCommand
    {
        public string UserName { get; set; }
    }

    public class MustBeValidUserNameValidator : AbstractValidator<MustBeValidUserNameCommand>
    {
        public MustBeValidUserNameValidator()
        {
            RuleFor(x => x.UserName)
                .MustBeValidUserName()
                .WithName(User.Constraints.NameLabel)
            ;
        }
    }
}