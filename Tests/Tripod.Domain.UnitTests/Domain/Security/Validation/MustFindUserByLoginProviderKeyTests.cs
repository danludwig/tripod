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
    public class MustFindUserByLoginProviderKeyTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () =>new MustFindUserByLoginProviderKey<object>(null, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenLoginProvider_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustFindUserByLoginProviderKey<object>(queries.Object, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("loginProvider");
        }

        [Fact]
        public void IsInvalid_WhenUser_IsNotFound_ByUserLoginInfo()
        {
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByLoginProviderKeyCommand
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey,
            };
            Expression<Func<UserBy, bool>> expectedQuery = x => x.UserLoginInfo.LoginProvider == loginProvider
                && x.UserLoginInfo.ProviderKey == providerKey;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as User));
            var validator = new FakeMustFindUserByLoginProviderKeyValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> emailIdError = x => x.PropertyName == command.PropertyName(y => y.ProviderKey);
            result.Errors.Count(emailIdError).ShouldEqual(1);
            result.Errors.Single(emailIdError).ErrorMessage.ShouldEqual(Resources.Validation_NoUserByLoginProviderKey
                .Replace("{PropertyName}", RemoteMembership.Constraints.ProviderUserIdLabel)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.ProviderKey, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenUser_IsFound_ByUserLoginInfo()
        {
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByLoginProviderKeyCommand
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey,
            };
            var user = new ProxiedUser(new Random().Next(1, int.MaxValue));
            user.RemoteMemberships.Add(new ProxiedRemoteMembership(loginProvider, providerKey)
            {
                User = user,
                UserId = user.Id,
            });
            Expression<Func<UserBy, bool>> expectedQuery = x => x.UserLoginInfo.LoginProvider == loginProvider
                && x.UserLoginInfo.ProviderKey == providerKey;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(user as User));
            var validator = new FakeMustFindUserByLoginProviderKeyValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.ProviderKey, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustFindUserByLoginProviderKeyCommand
    {
        public string ProviderKey { get; set; }
        public string LoginProvider { get; set; }
    }

    public class FakeMustFindUserByLoginProviderKeyValidator : AbstractValidator<FakeMustFindUserByLoginProviderKeyCommand>
    {
        public FakeMustFindUserByLoginProviderKeyValidator(IProcessQueries queries)
        {
            RuleFor(x => x.ProviderKey)
                .MustFindUserByLoginProviderKey(queries, x => x.LoginProvider)
                .WithName(RemoteMembership.Constraints.ProviderUserIdLabel)
            ;
        }
    }
}
