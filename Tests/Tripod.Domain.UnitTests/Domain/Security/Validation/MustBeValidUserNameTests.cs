using System;
using System.Globalization;
using System.Linq;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class MustBeValidUserNameTests : FluentValidationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\r\n")]
        public void IsInvalid_WhenUserName_IsEmpty(string userName)
        {
            var command = new FakeMustBeValidUserNameCommand
            {
                UserName = userName,
            };
            var validator = new FakeMustBeValidUserNameValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> userNameError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(userNameError).ShouldEqual(1);
            result.Errors.Single(userNameError).ErrorMessage.ShouldEqual(Resources.notempty_error
                .Replace("{PropertyName}", User.Constraints.NameLabel)
            );
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command.UserName);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("B")]
        public void IsInvalid_WhenUserName_IsLessThan_MinLength(string userName)
        {
            var command = new FakeMustBeValidUserNameCommand
            {
                UserName = userName,
            };
            var validator = new FakeMustBeValidUserNameValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> userNameError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(userNameError).ShouldEqual(1);
            result.Errors.Single(userNameError).ErrorMessage.ShouldEqual(Resources.Validation_MinLength
                .Replace("{PropertyName}", User.Constraints.NameLabel)
                .Replace("{MinLength}", User.Constraints.NameMinLength
                    .ToString(CultureInfo.InvariantCulture))
                .Replace("{TotalLength}", userName.Length.ToString(CultureInfo.InvariantCulture))
                .Replace("{Characters}", userName.Length == 1
                    ? Resources.Validation_CharacterLower
                    : Resources.Validation_CharactersLower)
            );
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command.UserName);
        }

        [Fact]
        public void IsInvalid_WhenUserName_IsGreaterThan_MaxLength()
        {
            var userName = Guid.NewGuid().ToString();
            while (userName.Length < User.Constraints.NameMaxLength)
                userName += Guid.NewGuid().ToString();
            var command = new FakeMustBeValidUserNameCommand
            {
                UserName = userName,
            };
            var validator = new FakeMustBeValidUserNameValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> userNameError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(userNameError).ShouldEqual(1);
            result.Errors.Single(userNameError).ErrorMessage.ShouldEqual(Resources.Validation_MaxLength
                .Replace("{PropertyName}", User.Constraints.NameLabel)
                .Replace("{MaxLength}", User.Constraints.NameMaxLength
                    .ToString(CultureInfo.InvariantCulture))
                .Replace("{TotalLength}", userName.Length.ToString(CultureInfo.InvariantCulture))
            );
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command.UserName);
        }

        [Theory]
        [InlineData("asdf&asdf")]
        [InlineData("a(*^sdf")]
        [InlineData("asdf\r\nasdf")]
        public void IsInvalid_WhenUserName_ContainsUnallowedCharacters(string userName)
        {
            var command = new FakeMustBeValidUserNameCommand
            {
                UserName = userName,
            };
            var validator = new FakeMustBeValidUserNameValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> userNameError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(userNameError).ShouldEqual(1);
            result.Errors.Single(userNameError).ErrorMessage.ShouldEqual(Resources.Validation_UserName_AllowedCharacters
                .Replace("{PropertyName}", User.Constraints.NameLabel)
            );
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command.UserName);
        }

        [Fact]
        public void IsValid_WhenUserName_IsEmailAddress()
        {
            var userName = string.Format("{0}@domain.tld", Guid.NewGuid());
            var command = new FakeMustBeValidUserNameCommand
            {
                UserName = userName,
            };
            var validator = new FakeMustBeValidUserNameValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command.UserName);
        }
    }
}
