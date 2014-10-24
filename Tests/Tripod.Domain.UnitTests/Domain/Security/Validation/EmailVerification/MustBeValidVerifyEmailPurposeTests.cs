using System;
using System.Globalization;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class MustBeValidVerifyEmailPurposeTests : FluentValidationTests
    {
        [Fact]
        public void IsInvalid_WhenPurpose_IsInvalid()
        {
            var command = new FakeMustBeValidVerifyEmailPurposeCommand { Purpose = EmailVerificationPurpose.Invalid };
            var validator = new FakeMustBeValidVerifyEmailPurposeValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> expectedError = x => x.PropertyName == command.PropertyName(y => y.Purpose);
            result.Errors.Count(expectedError).ShouldEqual(1);
            result.Errors.Single(expectedError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationPurpose_IsEmpty
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
            validator.ShouldHaveValidationErrorFor(x => x.Purpose, command.Purpose);
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.AddEmail)]
        [InlineData(EmailVerificationPurpose.CreateLocalUser)]
        [InlineData(EmailVerificationPurpose.CreateRemoteUser)]
        [InlineData(EmailVerificationPurpose.ForgotPassword)]
        public void IsValid_WhenPurpose_IsNotInvalid(EmailVerificationPurpose purpose)
        {
            var command = new FakeMustBeValidVerifyEmailPurposeCommand { Purpose = purpose };
            var validator = new FakeMustBeValidVerifyEmailPurposeValidator();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.Purpose, command.Purpose);
        }
    }

    public class FakeMustBeValidVerifyEmailPurposeCommand
    {
        public EmailVerificationPurpose Purpose { get; set; }
    }

    public class FakeMustBeValidVerifyEmailPurposeValidator : AbstractValidator<FakeMustBeValidVerifyEmailPurposeCommand>
    {
        public FakeMustBeValidVerifyEmailPurposeValidator()
        {
            RuleFor(x => x.Purpose)
                .MustBeValidVerifyEmailPurpose()
                .WithName(EmailVerification.Constraints.Label);
        }
    }
}
