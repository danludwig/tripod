using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeMustBeValidPasswordCommand
    {
        public string Password { get; set; }
    }

    public class FakeMustBeValidPasswordValidator : AbstractValidator<FakeMustBeValidPasswordCommand>
    {
        public FakeMustBeValidPasswordValidator()
        {
            RuleFor(x => x.Password)
                .MustBeValidPassword()
                .WithName(LocalMembership.Constraints.PasswordLabel);
        }
    }
}
