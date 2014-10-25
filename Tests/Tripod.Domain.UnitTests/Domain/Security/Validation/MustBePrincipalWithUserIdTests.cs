using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class MustBePrincipalWithUserIdTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenUserId_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustBePrincipalWithUserId<object>(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("userId");
        }

        [Fact]
        public void Validate_ThrowsNullReferenceException_WhenPrincipal_IsNull()
        {
            var userId = FakeData.Id();
            var command = new FakeMustBePrincipalWithUserIdCommand
            {
                Principal = null,
                UserId = userId,
            };
            var validator = new FakeMustBePrincipalWithUserIdValidator();
            var exception = Assert.Throws<NullReferenceException>(
                () => validator.Validate(command));
            exception.ShouldNotBeNull();
        }

        [Fact]
        public void Validate_ThrowsArgumentNullException_WhenPrincipalIdentity_IsNull()
        {
            var userId = FakeData.Id();
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            principal.SetupGet(x => x.Identity).Returns(null as IIdentity);
            var command = new FakeMustBePrincipalWithUserIdCommand
            {
                Principal = principal.Object,
                UserId = userId,
            };
            var validator = new FakeMustBePrincipalWithUserIdValidator();
            var exception = Assert.Throws<ArgumentNullException>(
                () => validator.Validate(command));
            exception.ShouldNotBeNull();
        }

        [Fact]
        public void IsInvalid_WhenPrincipalIdentity_IsNotClaimsIdentityInstance()
        {
            var userId = FakeData.Id();
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            principal.SetupGet(x => x.Identity).Returns(new GenericIdentity("username"));
            var command = new FakeMustBePrincipalWithUserIdCommand
            {
                Principal = principal.Object,
                UserId = userId,
            };
            var validator = new FakeMustBePrincipalWithUserIdValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> principalError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(principalError).ShouldEqual(1);
            result.Errors.Single(principalError).ErrorMessage.ShouldEqual(Resources.Validation_NotAuthorized_UserAction
                .Replace("{PropertyName}", User.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", principal.Object.Identity.Name)
                .Replace("{UserId}", command.UserId.ToString(CultureInfo.InvariantCulture))
            );
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command);
        }

        [Fact]
        public void IsInvalid_WhenPrincipalIdentity_IsClaimsIdentityInstance_WithDifferentNameIdentifierClaim()
        {
            var userId = FakeData.Id();
            var otherUserId = FakeData.Id(userId);
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, otherUserId.ToString(CultureInfo.InvariantCulture)),
            });
            principal.SetupGet(x => x.Identity).Returns(identity);
            var command = new FakeMustBePrincipalWithUserIdCommand
            {
                Principal = principal.Object,
                UserId = userId,
            };
            var validator = new FakeMustBePrincipalWithUserIdValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> principalError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(principalError).ShouldEqual(1);
            result.Errors.Single(principalError).ErrorMessage.ShouldEqual(Resources.Validation_NotAuthorized_UserAction
                .Replace("{PropertyName}", User.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", principal.Object.Identity.Name)
                .Replace("{UserId}", command.UserId.ToString(CultureInfo.InvariantCulture))
            );
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command);
        }

        [Fact]
        public void IsValid_WhenPrincipalIdentity_IsClaimsIdentityInstance_WithEqualNameIdentifierClaim()
        {
            var userId = FakeData.Id();
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, (userId).ToString(CultureInfo.InvariantCulture)),
            });
            principal.SetupGet(x => x.Identity).Returns(identity);
            var command = new FakeMustBePrincipalWithUserIdCommand
            {
                Principal = principal.Object,
                UserId = userId,
            };
            var validator = new FakeMustBePrincipalWithUserIdValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command);
        }
    }

    public class FakeMustBePrincipalWithUserIdCommand
    {
        public IPrincipal Principal { get; set; }
        public int UserId { get; set; }
    }

    public class FakeMustBePrincipalWithUserIdValidator : AbstractValidator<FakeMustBePrincipalWithUserIdCommand>
    {
        public FakeMustBePrincipalWithUserIdValidator()
        {
            RuleFor(x => x.Principal)
                .MustBePrincipalWithUserId(x => x.UserId)
                .WithName(User.Constraints.Label)
            ;
        }
    }
}
