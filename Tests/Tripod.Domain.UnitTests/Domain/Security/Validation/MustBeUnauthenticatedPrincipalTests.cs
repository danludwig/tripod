using System;
using System.Linq;
using System.Security.Principal;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class MustBeUnauthenticatedPrincipalTests : FluentValidationTests
    {
        [Fact]
        public void IsInvalid_WhenPrincipalIdentity_IsAuthenticated()
        {
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new Mock<IIdentity>(MockBehavior.Strict);
            identity.SetupGet(x => x.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity).Returns(identity.Object);
            var command = new FakeMustBeUnauthenticatedPrincipalCommand
            {
                Principal = principal.Object,
            };
            var validator = new FakeMustBeUnauthenticatedPrincipalValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> principalError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(principalError).ShouldEqual(1);
            result.Errors.Single(principalError).ErrorMessage.ShouldEqual(
                Resources.Validation_Principal_MustBeUnauthenticated
            );
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command);
        }

        [Fact]
        public void IsValid_WhenPrincipal_IsNull()
        {
            var command = new FakeMustBeUnauthenticatedPrincipalCommand();
            var validator = new FakeMustBeUnauthenticatedPrincipalValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command);
        }

        [Fact]
        public void IsValid_WhenPrincipalIdentity_IsNotAuthenticated()
        {
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var identity = new Mock<IIdentity>(MockBehavior.Strict);
            identity.SetupGet(x => x.IsAuthenticated).Returns(false);
            principal.SetupGet(x => x.Identity).Returns(identity.Object);
            var command = new FakeMustBeUnauthenticatedPrincipalCommand
            {
                Principal = principal.Object,
            };
            var validator = new FakeMustBeUnauthenticatedPrincipalValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command);
        }
    }
}
