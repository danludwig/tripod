using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class ChangeUserNameTests : FluentValidationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Validator_IsInvalid_WhenUserNameIsEmpty(string userName)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(null as User));
            var validator = new ValidateChangeUserNameCommand(queries.Object);
            var command = new ChangeUserName();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .notempty_error
                .Replace("{PropertyName}", User.Constraints.NameLabel));
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command);
        }

        [Fact]
        public void Validator_IsInvalid_WhenUserName_IsUnverifiedEmail()
        {
            var command = new ChangeUserName
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, FakeData.String()),
                    new Claim(ClaimTypes.NameIdentifier, FakeData.Id().ToString(CultureInfo.InvariantCulture)),
                }, "authenticationType"), null),
                UserName = FakeData.Email(),
                UserId = FakeData.Id(),
            };
            User user = new ProxiedUser(FakeData.Id());
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            EmailAddress emailAddress = new ProxiedEmailAddress(FakeData.Id())
            {
                UserId = user.Id,
                IsVerified = true,
                Value = FakeData.Email(),
            };
            var data = new[] { emailAddress }.AsQueryable();
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            Expression<Func<EmailAddressesBy, bool>> expectedQuery = x => x.UserId == command.UserId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(entitySet.AsQueryable()));
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(null as User));
            var validator = new ValidateChangeUserNameCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_UserName_AllowedEmailAddress
                .Replace("{PropertyName}", User.Constraints.NameLabel.ToLower())
                .Replace("{PropertyValue}", command.UserName)
            );
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command);
        }

        [Fact]
        public void Validator_IsInvalid_WhenUserName_IsFoundWithNonEqualUserId()
        {
            var command = new ChangeUserName
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, FakeData.String()),
                    new Claim(ClaimTypes.NameIdentifier, FakeData.Id().ToString(CultureInfo.InvariantCulture)),
                }, "authenticationType"), null),
                UserName = FakeData.Email(),
                UserId = FakeData.Id(),
            };
            User user = new ProxiedUser(FakeData.Id());
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            EmailAddress emailAddress = new ProxiedEmailAddress(FakeData.Id())
            {
                UserId = user.Id,
                IsVerified = true,
                Value = command.UserName,
            };
            var data = new[] { emailAddress }.AsQueryable();
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            Expression<Func<EmailAddressesBy, bool>> expectedQuery = x => x.UserId == command.UserId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(entitySet.AsQueryable()));
            queries.Setup(x => x.Execute(It.IsAny<UserBy>()))
                .Returns(Task.FromResult(new ProxiedUser(FakeData.Id()) as User));
            var validator = new ValidateChangeUserNameCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_AlreadyExists
                .Replace("{PropertyName}", User.Constraints.NameLabel)
                .Replace("{PropertyValue}", command.UserName)
            );
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command);
        }

        [Fact]
        public void Validator_IsInvalid_WhenPrincipalIdentityId_IsNotUserId()
        {
            var command = new ChangeUserName
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, FakeData.String()),
                    new Claim(ClaimTypes.NameIdentifier, FakeData.Id().ToString(CultureInfo.InvariantCulture)),
                }, "authenticationType"), null),
            };
            User user = new ProxiedUser(FakeData.Id());
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(user));
            var validator = new ValidateChangeUserNameCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_NotAuthorized_UserAction
                .Replace("{PropertyName}", User.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", command.Principal.Identity.Name)
                .Replace("{UserId}", command.UserId.ToString(CultureInfo.InvariantCulture))
            );
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command);
        }

        [Fact]
        public void Handler_LoadsUser_ByCommandUserId()
        {
            var command = new ChangeUserName
            {
                UserId = FakeData.Id(),
            };
            User user = new ProxiedUser(command.UserId);
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            Expression<Func<int, bool>> expectedUserId = x => x == command.UserId;
            entities.Setup(x => x.GetAsync<User>(It.Is(expectedUserId)))
                .Returns(Task.FromResult(user));
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            authenticator.Setup(x => x.SignOut()).Returns(Task.FromResult(0));
            authenticator.Setup(x => x.SignOn(It.IsAny<User>(), It.IsAny<bool>())).Returns(Task.FromResult(0));
            var handler = new HandleChangeUserNameCommand(entities.Object, authenticator.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.GetAsync<User>(It.Is(expectedUserId)), Times.Once);
        }
    }
}
