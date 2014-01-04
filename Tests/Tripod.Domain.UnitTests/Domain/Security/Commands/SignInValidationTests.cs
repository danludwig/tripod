using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class SignInValidationTests : FluentValidationTests
    {
        [Theory, InlineData(null), InlineData(""), InlineData("\t  \r\n")]
        public void IsInvalid_WhenUserName_IsEmpty(string userName)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserName = userName };

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}", User.Constraints.NameLabel));
            queries.Verify(x => x.Execute(It.IsAny<IsPasswordVerified>()), Times.Never);
        }

        [Theory, InlineData(null), InlineData(""), InlineData("\t  \r\n")]
        public void IsInvalid_WhenPassword_IsEmpty(string password)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { Password = password };

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Password);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}", LocalMembership.Constraints.PasswordLabel));
            queries.Verify(x => x.Execute(It.IsAny<IsPasswordVerified>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenPassword_IsNotVerified()
        {
            const string userName = "username";
            const string password = "wrong password";
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserName = userName, Password = password, };
            Expression<Func<IsPasswordVerified, bool>> expectedQuery = x =>
                x.UserName == command.UserName && x.Password == command.Password;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(false));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Password);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources.Validation_InvalidUsernameOrPassword);
            queries.Verify(x => x.Execute(It.IsAny<IsPasswordVerified>()), Times.Once);
        }

        [Fact]
        public void IsValid_WhenAllRulesPass()
        {
            const string userName = "username";
            const string password = "correct password";
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserName = userName, Password = password, };
            Expression<Func<IsPasswordVerified, bool>> expectedQuery = x =>
                x.UserName == command.UserName && x.Password == command.Password;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(true));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<IsPasswordVerified>()), Times.Once);
        }
    }
}
