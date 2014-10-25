using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class MustFindUserByPrincipalTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustFindUserByPrincipal(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenPrincipal_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByPrincipalCommand();
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(new User()));
            var validator = new FakeMustFindUserByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_DoesNotExist
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", "")
            );
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenPrincipal_IsNotAuthenticated()
        {
            string userName = Guid.NewGuid().ToString();
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new Mock<IIdentity>(MockBehavior.Strict);
            identity.SetupGet(x => x.IsAuthenticated).Returns(false);
            identity.SetupGet(x => x.Name).Returns(userName);
            principal.SetupGet(x => x.Identity).Returns(identity.Object);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByPrincipalCommand { Principal = principal.Object, };
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(new User()));
            var validator = new FakeMustFindUserByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_DoesNotExist
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", userName)
            );
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.IsAny<UserBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenUserNotFound_ByPrincipal()
        {
            string userName = Guid.NewGuid().ToString();
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new Mock<IIdentity>(MockBehavior.Strict);
            identity.SetupGet(x => x.IsAuthenticated).Returns(true);
            identity.SetupGet(x => x.Name).Returns(userName);
            principal.SetupGet(x => x.Identity).Returns(identity.Object);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByPrincipalCommand { Principal = principal.Object, };
            Expression<Func<UserBy, bool>> expectedQuery = x => x.Principal == principal.Object;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as User));
            var validator = new FakeMustFindUserByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_DoesNotExist
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", userName)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenUserIsFound_ByPrincipal()
        {
            string userName = Guid.NewGuid().ToString();
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new Mock<IIdentity>(MockBehavior.Strict);
            identity.SetupGet(x => x.IsAuthenticated).Returns(true);
            identity.SetupGet(x => x.Name).Returns(userName);
            principal.SetupGet(x => x.Identity).Returns(identity.Object);
            var entity = new ProxiedUser(FakeData.Id()) { Name = userName };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByPrincipalCommand { Principal = principal.Object, };
            Expression<Func<UserBy, bool>> expectedQuery = x => x.Principal == principal.Object;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity as User));
            var validator = new FakeMustFindUserByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustFindUserByPrincipalCommand
    {
        public IPrincipal Principal { get; set; }
    }

    public class FakeMustFindUserByPrincipalValidator : AbstractValidator<FakeMustFindUserByPrincipalCommand>
    {
        public FakeMustFindUserByPrincipalValidator(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .MustFindUserByPrincipal(queries)
                .WithName(User.Constraints.Label)
            ;
        }
    }
}
