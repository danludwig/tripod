using System;
using System.Globalization;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class CreateUserValidationTests
    {
        public CreateUserValidationTests()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            ValidatorOptions.ResourceProviderType = typeof(Resources);
        }

        [Fact]
        public void IsInvalid_WhenName_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser();

            var result = validator.Validate(command);

            result.IsValid.ShouldEqual(false);
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Name);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}", User.Constraints.NameLabel));
            validator.ShouldHaveValidationErrorFor(x => x.Name, command.Name);
        }

        [Fact]
        public void IsInvalid_WhenName_IsEmptyString()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser { Name = "" };

            var result = validator.Validate(command);

            result.IsValid.ShouldEqual(false);
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Name);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}", User.Constraints.NameLabel));
            validator.ShouldHaveValidationErrorFor(x => x.Name, command.Name);
        }

        [Fact]
        public void IsInvalid_WhenName_IsWhitespaceString()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser { Name = "\t  \r\n" };

            var result = validator.Validate(command);

            result.IsValid.ShouldEqual(false);
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Name);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}", User.Constraints.NameLabel));
            validator.ShouldHaveValidationErrorFor(x => x.Name, command.Name);
        }

        [Fact]
        public void IsInvalid_WhenName_LengthIsLessThan2()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser { Name = "i" };

            var result = validator.Validate(command);

            result.IsValid.ShouldEqual(false);
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Name);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_MinLength
                .Replace("{PropertyName}", User.Constraints.NameLabel)
                .Replace("{MinLength}", User.Constraints.NameMinLength.ToString(CultureInfo.InvariantCulture))
                .Replace("{TotalLength}", command.Name.Length.ToString(CultureInfo.InvariantCulture))
            );
            validator.ShouldHaveValidationErrorFor(x => x.Name, command.Name);
        }
    }
}
