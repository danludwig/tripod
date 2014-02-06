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
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}",
                    string.Format("{0} or {1}", User.Constraints.NameLabel, EmailAddress.Constraints.Label)));
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenUserName_MatchesNoUserOrEmail()
        {
            const string userName = "noMatchUserName";
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserName = userName };
            Expression<Func<UserBy, bool>> userByName = x => x.Name == command.UserName;
            Expression<Func<EmailAddressBy, bool>> emailByValue = x => x.Value == command.UserName;
            queries.Setup(x => x.Execute(It.Is(userByName))).Returns(Task.FromResult(null as User));
            queries.Setup(x => x.Execute(It.Is(emailByValue))).Returns(Task.FromResult(null as EmailAddress));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage
                .ShouldEqual(Resources.Validation_CouldNotFind
                    .Replace("{PropertyName}", string.Format("{0} or {1}", User.Constraints.NameLabel, EmailAddress.Constraints.Label).ToLower())
                    .Replace("{PropertyValue}", command.UserName));
            queries.Verify(x => x.Execute(It.Is(userByName)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(emailByValue)), Times.Once);
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
        public void IsInvalid_WhenPassword_IsNotVerified_AndUserExists()
        {
            const string userName = "username";
            const string password = "wrong password";
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserName = userName, Password = password, };
            Expression<Func<IsPasswordVerified, bool>> expectedQuery = x =>
                x.UserName == command.UserName && x.Password == command.Password;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(false));
            Expression<Func<UserBy, bool>> userQuery = x => x.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(userQuery))).Returns(Task.FromResult(new User()));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Password);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources.Validation_InvalidPassword);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
        }

        [Fact]
        public void PasswordIsValid_WhenNoUserExists()
        {
            const string userName = "username";
            const string password = "wrong password";
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserName = userName, Password = password, };
            Expression<Func<IsPasswordVerified, bool>> expectedQuery = x =>
                x.UserName == command.UserName && x.Password == command.Password;
            Expression<Func<UserBy, bool>> userByName = x => x.Name == command.UserName;
            Expression<Func<EmailAddressBy, bool>> emailByValue = x => x.Value == command.UserName;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(false));
            queries.Setup(x => x.Execute(It.Is(userByName))).Returns(Task.FromResult(null as User));
            queries.Setup(x => x.Execute(It.Is(emailByValue))).Returns(Task.FromResult(null as EmailAddress));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Password);
            result.Errors.Count(targetError).ShouldEqual(0);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Never);
        }

        [Fact]
        public void IsValid_WhenAllRulesPass()
        {
            const string userName = "username";
            const string password = "correct password";
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserName = userName, Password = password, };
            Expression<Func<IsPasswordVerified, bool>> passwordQuery = x =>
                x.UserName == command.UserName && x.Password == command.Password;
            queries.Setup(x => x.Execute(It.Is(passwordQuery))).Returns(Task.FromResult(true));
            Expression<Func<UserBy, bool>> userQuery = x => x.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(userQuery))).Returns(Task.FromResult(new User()));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(passwordQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(userQuery)), Times.AtLeastOnce);
        }
    }
}
