using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class MustBeVerifiedPasswordTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () =>new MustBeVerifiedPassword<object>(null, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenUserName_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustBeVerifiedPassword<object>(queries.Object, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("userName");
        }

        [Fact]
        public void IsInvalid_WhenPassword_IsNotVerified()
        {
            string userName = FakeData.String();
            string password = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiedPasswordCommand
            {
                UserName = userName,
                Password = password,
            };
            Expression<Func<IsPasswordVerified, bool>> expectedQuery =
                x => x.UserNameOrVerifiedEmail == userName && x.Password == password;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(false));
            var validator = new FakeMustBeVerifiedPasswordValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> passwordError = x => x.PropertyName == command.PropertyName(y => y.Password);
            result.Errors.Count(passwordError).ShouldEqual(1);
            result.Errors.Single(passwordError).ErrorMessage.ShouldEqual(
                Resources.Validation_InvalidPassword
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Password, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenPassword_IsVerified()
        {
            string userName = FakeData.String();
            string password = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiedPasswordCommand
            {
                UserName = userName,
                Password = password,
            };
            Expression<Func<IsPasswordVerified, bool>> expectedQuery =
                x => x.UserNameOrVerifiedEmail == userName && x.Password == password;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(true));
            var validator = new FakeMustBeVerifiedPasswordValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Password, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

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
