using System;
using System.Linq;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class MustNotContainInvalidUserNameTextTests : FluentValidationTests
    {
        [Theory]
        [InlineData("asdf&asdf")]
        [InlineData("a(*^sdf")]
        [InlineData("asdf\r\nasdf")]
        public void IsInvalid_WhenUserName_ContainsUnallowedCharacters(string userName)
        {
            var command = new MustNotContainInvalidUserNameTextCommand
            {
                UserName = userName,
            };
            var validator = new MustNotContainInvalidUserNameTextValidator();

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
            var command = new MustNotContainInvalidUserNameTextCommand
            {
                UserName = userName,
            };
            var validator = new MustNotContainInvalidUserNameTextValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command.UserName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValid_WhenUserName_IsEmpty(string userName)
        {
            var command = new MustNotContainInvalidUserNameTextCommand
            {
                UserName = userName,
            };
            var validator = new MustNotContainInvalidUserNameTextValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command.UserName);
        }
    }
}
