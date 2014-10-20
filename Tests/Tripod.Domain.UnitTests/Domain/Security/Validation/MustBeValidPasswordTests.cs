using System;
using System.Globalization;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class MustBeValidPasswordTests : FluentValidationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("        \r\n\t     ")]
        public void IsInvalid_WhenPassword_IsEmpty(string password)
        {
            var command = new FakeMustBeValidPasswordCommand { Password = password };
            var validator = new FakeMustBeValidPasswordValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> passwordError = x => x.PropertyName == command.PropertyName(y => y.Password);
            result.Errors.Count(passwordError).ShouldEqual(1);
            result.Errors.Single(passwordError).ErrorMessage.ShouldEqual(Resources.notempty_error
                .Replace("{PropertyName}", LocalMembership.Constraints.PasswordLabel)
            );
            validator.ShouldHaveValidationErrorFor(x => x.Password, command.Password);
        }

        [Theory]
        [InlineData("1234567")]
        [InlineData("asdf")]
        [InlineData("x")]
        public void IsInvalid_WhenPasswordLength_IsLessThan_PasswordMinLength(string password)
        {
            var command = new FakeMustBeValidPasswordCommand { Password = password };
            var validator = new FakeMustBeValidPasswordValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> passwordError = x => x.PropertyName == command.PropertyName(y => y.Password);
            result.Errors.Count(passwordError).ShouldEqual(1);
            result.Errors.Single(passwordError).ErrorMessage.ShouldEqual(Resources.Validation_MinLength
                .Replace("{PropertyName}", LocalMembership.Constraints.PasswordLabel)
                .Replace("{MinLength}", LocalMembership.Constraints.PasswordMinLength
                    .ToString(CultureInfo.InvariantCulture))
                .Replace("{TotalLength}", password.Length.ToString(CultureInfo.InvariantCulture))
                .Replace("{Characters}", password.Length == 1
                    ? Resources.Validation_CharacterLower
                    : Resources.Validation_CharactersLower)
            );
            validator.ShouldHaveValidationErrorFor(x => x.Password, command.Password);
        }

        [Fact]
        public void IsInvalid_WhenPasswordLength_IsGreaterThan_PasswordMaxLength()
        {
            var password = Guid.NewGuid().ToString();
            while (password.Length <= LocalMembership.Constraints.PasswordMaxLength)
                password += Guid.NewGuid().ToString();
            var command = new FakeMustBeValidPasswordCommand { Password = password };
            var validator = new FakeMustBeValidPasswordValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> passwordError = x => x.PropertyName == command.PropertyName(y => y.Password);
            result.Errors.Count(passwordError).ShouldEqual(1);
            result.Errors.Single(passwordError).ErrorMessage.ShouldEqual(Resources.Validation_MaxLength
                .Replace("{PropertyName}", LocalMembership.Constraints.PasswordLabel)
                .Replace("{MaxLength}", LocalMembership.Constraints.PasswordMaxLength
                    .ToString(CultureInfo.InvariantCulture))
                .Replace("{TotalLength}", password.Length.ToString(CultureInfo.InvariantCulture))
            );
            validator.ShouldHaveValidationErrorFor(x => x.Password, command.Password);
        }

        [Theory]
        [InlineData("123456789")]
        [InlineData("asdfasdf")]
        [InlineData("xxxx#$Yz46")]
        public void IsValid_WhenPasswordLength_IsInValidRange(string password)
        {
            var command = new FakeMustBeValidPasswordCommand { Password = password };
            var validator = new FakeMustBeValidPasswordValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.Password, command.Password);
        }
    }

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
