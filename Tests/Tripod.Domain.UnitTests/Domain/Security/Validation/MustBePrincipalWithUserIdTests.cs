using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
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
            const int userId = 11;
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
            const int userId = 11;
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
            const int userId = 11;
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
            const int userId = 11;
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new ClaimsIdentity("username");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, (userId + 6).ToString(CultureInfo.InvariantCulture)));
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
            const int userId = 11;
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new ClaimsIdentity("username");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)));
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
}
