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
    public class MustNotFindLocalMembershipByPrincipalTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustNotFindLocalMembershipByPrincipal(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenLocalMembershipIsFound_ByPrincipal()
        {
            var userId = FakeData.Id();
            var userName = Guid.NewGuid().ToString();
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture))
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ProxiedUser(userId) { Name = userName, };
            var localMembership = new LocalMembership { User = user, };
            principal.SetupGet(x => x.Identity).Returns(identity);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindLocalMembershipByPrincipalCommand { Principal = principal.Object };
            Expression<Func<LocalMembershipByUser, bool>> expectedQuery = x => x.UserId == userId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(localMembership));
            var validator = new FakeMustNotFindLocalMembershipByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> expectedError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(expectedError).ShouldEqual(1);
            result.Errors.Single(expectedError).ErrorMessage.ShouldEqual(Resources
                .Validation_LocalMembershipByUser_AlreadyExists
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", userName)
                .Replace("{PasswordLabel}", LocalMembership.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenPrincipal_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindLocalMembershipByPrincipalCommand();
            queries.Setup(x => x.Execute(It.IsAny<LocalMembershipByUser>()))
                .Returns(Task.FromResult(new LocalMembership()));
            var validator = new FakeMustNotFindLocalMembershipByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
        }

        [Fact]
        public void IsValid_WhenPrincipalIdentity_IsNotClaimsIdentity()
        {
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new Mock<IIdentity>(MockBehavior.Strict);
            principal.SetupGet(x => x.Identity).Returns(identity.Object);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindLocalMembershipByPrincipalCommand { Principal = principal.Object };
            queries.Setup(x => x.Execute(It.IsAny<LocalMembershipByUser>()))
                .Returns(Task.FromResult(new LocalMembership()));
            var validator = new FakeMustNotFindLocalMembershipByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
        }

        [Fact]
        public void IsValid_WhenPrincipalIdentity_HasNoNameIdentifierClaim()
        {
            var userId = FakeData.Id();
            var userName = Guid.NewGuid().ToString();
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.PostalCode, userId.ToString(CultureInfo.InvariantCulture))
            };
            var identity = new ClaimsIdentity(claims);
            principal.SetupGet(x => x.Identity).Returns(identity);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindLocalMembershipByPrincipalCommand { Principal = principal.Object };
            queries.Setup(x => x.Execute(It.IsAny<LocalMembershipByUser>()))
                .Returns(Task.FromResult(new LocalMembership()));
            var validator = new FakeMustNotFindLocalMembershipByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.IsAny<LocalMembershipByUser>()), Times.Never);
        }

        [Fact]
        public void IsValid_WhenLocalMembershipIsNotFound_ByPrincipal()
        {
            var userId = FakeData.Id();
            var userName = Guid.NewGuid().ToString();
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture))
            };
            var identity = new ClaimsIdentity(claims);
            principal.SetupGet(x => x.Identity).Returns(identity);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindLocalMembershipByPrincipalCommand { Principal = principal.Object };
            Expression<Func<LocalMembershipByUser, bool>> expectedQuery = x => x.UserId == userId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(null as LocalMembership));
            var validator = new FakeMustNotFindLocalMembershipByPrincipalValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustNotFindLocalMembershipByPrincipalCommand
    {
        public IPrincipal Principal { get; set; }
    }

    public class FakeMustNotFindLocalMembershipByPrincipalValidator :
        AbstractValidator<FakeMustNotFindLocalMembershipByPrincipalCommand>
    {
        public FakeMustNotFindLocalMembershipByPrincipalValidator(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .MustNotFindLocalMembershipByPrincipal(queries)
                .WithName(User.Constraints.Label)
            ;
        }
    }
}
