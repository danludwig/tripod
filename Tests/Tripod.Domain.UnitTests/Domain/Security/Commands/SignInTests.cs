using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class SignInTests : FluentValidationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t  \r\n")]
        public void Validator_IsInvalid_WhenUserName_IsEmpty(string userName)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserNameOrVerifiedEmail = userName };

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.UserNameOrVerifiedEmail);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}",
                    string.Format("{0} or {1}", User.Constraints.NameLabel, EmailAddress.Constraints.Label)));
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
        }

        [Fact]
        public void Validator_IsInvalid_WhenUserName_MatchesNoUserOrEmail()
        {
            var userName = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserNameOrVerifiedEmail = userName };
            Expression<Func<UserByNameOrVerifiedEmail, bool>> userById = x => x.NameOrEmail == command.UserNameOrVerifiedEmail;
            queries.Setup(x => x.Execute(It.Is(userById))).Returns(Task.FromResult(null as User));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.UserNameOrVerifiedEmail);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage
                .ShouldEqual(Resources.Validation_CouldNotFind
                    .Replace("{PropertyName}", string.Format("{0} or {1}", User.Constraints.NameLabel, EmailAddress.Constraints.Label).ToLower())
                    .Replace("{PropertyValue}", command.UserNameOrVerifiedEmail));
            queries.Verify(x => x.Execute(It.Is(userById)), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t  \r\n")]
        public void Validator_IsInvalid_WhenPassword_IsEmpty(string password)
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
        public void Validator_IsInvalid_WhenPassword_IsNotVerified_AndUserExists()
        {
            var userName = FakeData.String();
            var password = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserNameOrVerifiedEmail = userName, Password = password, };
            Expression<Func<IsPasswordVerified, bool>> expectedQuery = x =>
                x.UserNameOrVerifiedEmail == command.UserNameOrVerifiedEmail && x.Password == command.Password;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(false));
            Expression<Func<UserByNameOrVerifiedEmail, bool>> userQuery = x => x.NameOrEmail == command.UserNameOrVerifiedEmail;
            queries.Setup(x => x.Execute(It.Is(userQuery))).Returns(Task.FromResult(new User()));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Password);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources.Validation_InvalidPassword);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
        }

        [Fact]
        public void Validator_PasswordIsValid_WhenNoUserExists()
        {
            var userName = FakeData.String();
            var password = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserNameOrVerifiedEmail = userName, Password = password, };
            Expression<Func<IsPasswordVerified, bool>> expectedQuery = x =>
                x.UserNameOrVerifiedEmail == command.UserNameOrVerifiedEmail && x.Password == command.Password;
            Expression<Func<UserByNameOrVerifiedEmail, bool>> userById = x => x.NameOrEmail == command.UserNameOrVerifiedEmail;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(false));
            queries.Setup(x => x.Execute(It.Is(userById))).Returns(Task.FromResult(null as User));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Password);
            result.Errors.Count(targetError).ShouldEqual(0);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Never);
            queries.Verify(x => x.Execute(It.Is(userById)), Times.Exactly(2));
        }

        [Fact]
        public void Validator_IsValid_WhenAllRulesPass()
        {
            var userName = FakeData.String();
            var password = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSignInCommand(queries.Object);
            var command = new SignIn { UserNameOrVerifiedEmail = userName, Password = password, };
            Expression<Func<IsPasswordVerified, bool>> passwordQuery = x =>
                x.UserNameOrVerifiedEmail == command.UserNameOrVerifiedEmail && x.Password == command.Password;
            queries.Setup(x => x.Execute(It.Is(passwordQuery))).Returns(Task.FromResult(true));
            Expression<Func<UserByNameOrVerifiedEmail, bool>> userQuery = x => x.NameOrEmail == command.UserNameOrVerifiedEmail;
            queries.Setup(x => x.Execute(It.Is(userQuery))).Returns(Task.FromResult(new User()));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(passwordQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(userQuery)), Times.AtLeastOnce);
        }

        [Fact]
        public void Handler_FindsUser_ByNameAndPassword()
        {
            var command = new SignIn { UserNameOrVerifiedEmail = "username", Password = "password" };
            var user = new User { Name = command.UserNameOrVerifiedEmail };
            var userResult = Task.FromResult(user);
            var userStore = new Mock<IUserStore<User, int>>();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var userManager = new Mock<UserManager<User, int>>(userStore.Object);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            var handler = new HandleSignInCommand(queries.Object, userManager.Object, authenticator.Object);
            Expression<Func<UserByNameOrVerifiedEmail, bool>> userByNameOrVerifiedEmail = x => x.NameOrEmail == command.UserNameOrVerifiedEmail;
            queries.Setup(x => x.Execute(It.Is(userByNameOrVerifiedEmail))).Returns(userResult);
            userManager.Setup(x => x.FindAsync(command.UserNameOrVerifiedEmail, command.Password))
                .Returns(userResult);
            authenticator.Setup(x => x.SignOn(It.IsAny<User>(), It.IsAny<bool>())).Returns(Task.FromResult(0));

            handler.Handle(command).Wait();

            userManager.Verify(x => x.FindAsync(command.UserNameOrVerifiedEmail, command.Password), Times.Once);
        }

        [Fact]
        public void Handler_AuthenticatesUser()
        {
            var command = new SignIn { UserNameOrVerifiedEmail = "username", Password = "password" };
            var user = new User { Name = command.UserNameOrVerifiedEmail };
            var userResult = Task.FromResult(user);
            var userStore = new Mock<IUserStore<User, int>>();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var userManager = new Mock<UserManager<User, int>>(userStore.Object);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            var handler = new HandleSignInCommand(queries.Object, userManager.Object, authenticator.Object);
            Expression<Func<UserByNameOrVerifiedEmail, bool>> userByNameOrVerifiedEmail = x => x.NameOrEmail == command.UserNameOrVerifiedEmail;
            queries.Setup(x => x.Execute(It.Is(userByNameOrVerifiedEmail))).Returns(userResult);
            userManager.Setup(x => x.FindAsync(command.UserNameOrVerifiedEmail, command.Password)).Returns(userResult);
            authenticator.Setup(x => x.SignOn(user, command.IsPersistent)).Returns(Task.FromResult(0));

            handler.Handle(command).Wait();

            authenticator.Verify(x => x.SignOn(user, command.IsPersistent), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handler_AuthenticatesUser_UsingCommand_IsPersistent(bool isPersistent)
        {
            var command = new SignIn { UserNameOrVerifiedEmail = "username", Password = "password", IsPersistent = isPersistent };
            var user = new User { Name = command.UserNameOrVerifiedEmail };
            var userResult = Task.FromResult(user);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var userStore = new Mock<IUserStore<User, int>>();
            var userManager = new Mock<UserManager<User, int>>(userStore.Object);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            var handler = new HandleSignInCommand(queries.Object, userManager.Object, authenticator.Object);
            Expression<Func<UserByNameOrVerifiedEmail, bool>> userByNameOrVerifiedEmail = x => x.NameOrEmail == command.UserNameOrVerifiedEmail;
            queries.Setup(x => x.Execute(It.Is(userByNameOrVerifiedEmail))).Returns(userResult);
            userManager.Setup(x => x.FindAsync(command.UserNameOrVerifiedEmail, command.Password)).Returns(userResult);
            authenticator.Setup(x => x.SignOn(user, isPersistent)).Returns(Task.FromResult(0));

            handler.Handle(command).Wait();

            authenticator.Verify(x => x.SignOn(user, isPersistent), Times.Once);
        }
    }
}
