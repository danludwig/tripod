using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeMustEqualPasswordCommand
    {
        public string PasswordA { get; set; }
        public string PasswordB { get; set; }
        public bool UseMatchLabel { get; set; }
        public const string MatchLabelConstant = "other Password";
    }

    public class FakeMustEqualPasswordValidator : AbstractValidator<FakeMustEqualPasswordCommand>
    {
        public FakeMustEqualPasswordValidator()
        {
            When(x => !x.UseMatchLabel, () =>
                RuleFor(x => x.PasswordA)
                    .MustEqualPassword(x => x.PasswordB)
                    .WithName(LocalMembership.Constraints.PasswordLabel)
            );

            When(x => x.UseMatchLabel, () =>
                RuleFor(x => x.PasswordA)
                    .MustEqualPassword(x => x.PasswordB, FakeMustEqualPasswordCommand.MatchLabelConstant)
                    .WithName(LocalMembership.Constraints.PasswordLabel)
            );
        }
    }
}
