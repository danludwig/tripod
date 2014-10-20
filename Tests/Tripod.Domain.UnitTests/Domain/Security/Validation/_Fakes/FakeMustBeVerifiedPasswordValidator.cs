using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeMustBeVerifiedPasswordCommand
    {
        public string Password { get; set; }
        public string UserName { get; set; }
    }

    public class FakeMustBeVerifiedPasswordValidator : AbstractValidator<FakeMustBeVerifiedPasswordCommand>
    {
        public FakeMustBeVerifiedPasswordValidator(IProcessQueries queries)
        {
            RuleFor(x => x.Password)
                .MustBeVerifiedPassword(queries, x => x.UserName)
                .WithName(LocalMembership.Constraints.PasswordLabel);
        }
    }
}
