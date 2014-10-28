using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class ChangeLocalPasswordTests : FluentValidationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Validator_IsInvalid_WhenOldPasswordIsEmpty(string oldPassword)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateChangeLocalPasswordCommand(queries.Object);
            var command = new ChangeLocalPassword();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.OldPassword);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .notempty_error
                .Replace("{PropertyName}", LocalMembership.Constraints.OldPasswordLabel));
            queries.Verify(x => x.Execute(It.IsAny<IsPasswordVerified>()), Times.Never);
        }

        [Fact]
        public void Validator_IsInvalid_WhenOldPassword_IsNotTheRightPassword()
        {
            var command = new ChangeLocalPassword
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, FakeData.String()),
                    new Claim(ClaimTypes.NameIdentifier, FakeData.Id().ToString(CultureInfo.InvariantCulture)),
                }), null),
                OldPassword = FakeData.String(),
                NewPassword = FakeData.String(),
            };
            command.ConfirmPassword = command.NewPassword;
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<IsPasswordVerified, bool>> expectedQuery = x =>
                x.UserNameOrVerifiedEmail == command.Principal.Identity.Name &&
                x.Password == command.OldPassword;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(false));
            var validator = new ValidateChangeLocalPasswordCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.OldPassword);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_InvalidPassword
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
        }

        [Fact]
        public void Handler_LoadsUser_ByPrincipalIdentityUserId()
        {
            var command = new ChangeLocalPassword
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.Id().ToString(CultureInfo.InvariantCulture)),
                }), null),
                NewPassword = FakeData.String(),
            };
            User[] data =
            {
                new ProxiedUser(FakeData.Id()),
                new ProxiedUser(command.Principal.Identity.GetUserId<int>())
                {
                    LocalMembership = new LocalMembership(),
                },
                new ProxiedUser(FakeData.Id()),
            };
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Get<User>()).Returns(dbSet.Object);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var userStore = new Mock<IUserStore<User, int>>(MockBehavior.Strict);
            var userManager = new UserManager<User, int>(userStore.Object);
            var handler = new HandleChangeLocalPasswordCommand(entities.Object, userManager);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<User>(), Times.Once);
        }

        [Fact]
        public void Handler_SavesChanges()
        {
            var command = new ChangeLocalPassword
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.Id().ToString(CultureInfo.InvariantCulture)),
                }), null),
                NewPassword = FakeData.String(),
            };
            User[] data =
            {
                new ProxiedUser(FakeData.Id()),
                new ProxiedUser(command.Principal.Identity.GetUserId<int>())
                {
                    LocalMembership = new LocalMembership(),
                },
                new ProxiedUser(FakeData.Id()),
            };
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Get<User>()).Returns(dbSet.Object);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var userStore = new Mock<IUserStore<User, int>>(MockBehavior.Strict);
            var userManager = new UserManager<User, int>(userStore.Object);
            var handler = new HandleChangeLocalPasswordCommand(entities.Object, userManager);

            handler.Handle(command).Wait();

            entities.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
