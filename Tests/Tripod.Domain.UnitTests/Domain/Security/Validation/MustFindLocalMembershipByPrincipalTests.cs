using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
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
    public class MustFindLocalMembershipByPrincipalTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustFindLocalMembershipByPrincipal(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenPrincipal_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindLocalMembershipByPrincipalCommand();
            queries.Setup(x => x.Execute(It.IsAny<LocalMembershipByUser>()))
                .Returns(Task.FromResult(null as LocalMembership));
            var validator = new FakeMustFindLocalMembershipByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> principalError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(principalError).ShouldEqual(1);
            result.Errors.Single(principalError).ErrorMessage.ShouldEqual(
                Resources.Validation_LocalMembershipByUser_DoesNotExist
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", "")
                .Replace("{PasswordLabel}", LocalMembership.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenPrincipalIdentity_IsNotClaimsIdentity()
        {
            var userName = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new Mock<IIdentity>(MockBehavior.Strict);
            identity.SetupGet(x => x.Name).Returns(userName);
            principal.SetupGet(x => x.Identity).Returns(identity.Object);
            var command = new FakeMustFindLocalMembershipByPrincipalCommand { Principal = principal.Object, };
            queries.Setup(x => x.Execute(It.IsAny<LocalMembershipByUser>()))
                .Returns(Task.FromResult(null as LocalMembership));
            var validator = new FakeMustFindLocalMembershipByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> principalError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(principalError).ShouldEqual(1);
            result.Errors.Single(principalError).ErrorMessage.ShouldEqual(
                Resources.Validation_LocalMembershipByUser_DoesNotExist
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", userName)
                .Replace("{PasswordLabel}", LocalMembership.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenPrincipalIdentity_HasNoNameIdentifierClaim()
        {
            var userId = FakeData.Id();
            var userName = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var claims = new[]
            {
                new Claim(ClaimTypes.Locality, userId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, userName),
            };
            var identity = new ClaimsIdentity(claims);
            principal.SetupGet(x => x.Identity).Returns(identity);
            var command = new FakeMustFindLocalMembershipByPrincipalCommand { Principal = principal.Object, };
            queries.Setup(x => x.Execute(It.IsAny<LocalMembershipByUser>()))
                .Returns(Task.FromResult(null as LocalMembership));
            var validator = new FakeMustFindLocalMembershipByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> principalError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(principalError).ShouldEqual(1);
            result.Errors.Single(principalError).ErrorMessage.ShouldEqual(
                Resources.Validation_LocalMembershipByUser_DoesNotExist
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", userName)
                .Replace("{PasswordLabel}", LocalMembership.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenNotFound_ByPrincipal()
        {
            var userId = FakeData.Id();
            var userName = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, userName),
            };
            var identity = new ClaimsIdentity(claims);
            principal.SetupGet(x => x.Identity).Returns(identity);
            var command = new FakeMustFindLocalMembershipByPrincipalCommand { Principal = principal.Object, };
            Expression<Func<LocalMembershipByUser, bool>> expectedQuery = x => x.UserId == userId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(null as LocalMembership));
            var validator = new FakeMustFindLocalMembershipByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> principalError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(principalError).ShouldEqual(1);
            result.Errors.Single(principalError).ErrorMessage.ShouldEqual(
                Resources.Validation_LocalMembershipByUser_DoesNotExist
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", userName)
                .Replace("{PasswordLabel}", LocalMembership.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenFound_ByPrincipal()
        {
            var userId = FakeData.Id();
            var userName = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, userName),
            };
            var identity = new ClaimsIdentity(claims);
            principal.SetupGet(x => x.Identity).Returns(identity);
            var command = new FakeMustFindLocalMembershipByPrincipalCommand { Principal = principal.Object, };
            var user = new ProxiedUser(userId) { Name = userName };
            var localMembership = new LocalMembership { User = user };
            Expression<Func<LocalMembershipByUser, bool>> expectedQuery = x => x.UserId == userId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(localMembership));
            var validator = new FakeMustFindLocalMembershipByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustFindLocalMembershipByPrincipalCommand
    {
        public IPrincipal Principal { get; set; }
    }

    public class FakeMustFindLocalMembershipByPrincipalValidator : AbstractValidator<FakeMustFindLocalMembershipByPrincipalCommand>
    {
        public FakeMustFindLocalMembershipByPrincipalValidator(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .MustFindLocalMembershipByPrincipal(queries)
                .WithName(User.Constraints.Label)
            ;
        }
    }
}
