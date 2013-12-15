using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class CreateUserValidationTests : FluentValidationTests
    {
        [Fact]
        public void IsInvalid_WhenName_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Name);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}", User.Constraints.NameLabel));
            validator.ShouldHaveValidationErrorFor(x => x.Name, command.Name);
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenName_IsEmptyString()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser { Name = "" };

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Name);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}", User.Constraints.NameLabel));
            validator.ShouldHaveValidationErrorFor(x => x.Name, command.Name);
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenName_IsWhitespaceString()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser { Name = "\t  \r\n" };

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Name);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}", User.Constraints.NameLabel));
            validator.ShouldHaveValidationErrorFor(x => x.Name, command.Name);
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenNameLength_IsLessThan_MinLength()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser { Name = "i" };

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Name);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_MinLength
                .Replace("{PropertyName}", User.Constraints.NameLabel)
                .Replace("{MinLength}", User.Constraints.NameMinLength.ToString(CultureInfo.InvariantCulture))
                .Replace("{TotalLength}", command.Name.Length.ToString(CultureInfo.InvariantCulture))
            );
            validator.ShouldHaveValidationErrorFor(x => x.Name, command.Name);
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenNameLength_IsGreaterThan_MaxLength()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser { Name = string.Format("{0} {1} {2}", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()) };

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Name);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_MaxLength
                .Replace("{PropertyName}", User.Constraints.NameLabel)
                .Replace("{MaxLength}", User.Constraints.NameMaxLength.ToString(CultureInfo.InvariantCulture))
                .Replace("{TotalLength}", command.Name.Length.ToString(CultureInfo.InvariantCulture))
            );
            validator.ShouldHaveValidationErrorFor(x => x.Name, command.Name);
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenName_AlreadyExists()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser
            {
                Name = "alreadyIn",
            };
            Expression<Func<UserBy, bool>> expectedQuery = y => y.Name == command.Name;
            var entity = new User { Name = "AlreadyIn" };
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Name);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_AlreadyExists
                .Replace("{PropertyName}", User.Constraints.NameLabel)
                .Replace("{PropertyValue}", command.Name)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Name, command.Name);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenAllRulesPass()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateUserCommand(queries.Object);
            var command = new CreateUser { Name = "valid" };
            Expression<Func<UserBy, bool>> expectedQuery = y => y.Name == command.Name;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as User));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Name, command.Name);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }
}
