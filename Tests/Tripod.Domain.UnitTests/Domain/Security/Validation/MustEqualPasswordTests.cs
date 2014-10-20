using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class MustEqualPasswordTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenPassword_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustEqualPassword<object>(null, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("password");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsInvalid_WhenPasswords_DoNotMatch(bool useMatchLabel)
        {
            string passwordA = Guid.NewGuid().ToString();
            string passwordB = Guid.NewGuid().ToString();
            var command = new FakeMustEqualPasswordCommand
            {
                PasswordA = passwordA,
                PasswordB = passwordB,
                UseMatchLabel = useMatchLabel,
            };
            var validator = new FakeMustEqualPasswordValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> passwordError = x => x.PropertyName == command.PropertyName(y => y.PasswordA);
            result.Errors.Count(passwordError).ShouldEqual(1);
            result.Errors.Single(passwordError).ErrorMessage.ShouldEqual(
                Resources.Validation_PasswordDoesNotEqualConfirmation
                .Replace("{PropertyName}", LocalMembership.Constraints.PasswordLabel)
                .Replace("{PasswordLabel}", useMatchLabel
                    ? FakeMustEqualPasswordCommand.MatchLabelConstant
                    : LocalMembership.Constraints.PasswordLabel.ToLower())
            );
            validator.ShouldHaveValidationErrorFor(x => x.PasswordA, command);
        }

        [Fact]
        public void IsValid_WhenPasswords_Match()
        {
            string passwordA = Guid.NewGuid().ToString();
            string passwordB = passwordA;
            var command = new FakeMustEqualPasswordCommand
            {
                PasswordA = passwordA,
                PasswordB = passwordB,
            };
            var validator = new FakeMustEqualPasswordValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.PasswordA, command);
        }
    }

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
