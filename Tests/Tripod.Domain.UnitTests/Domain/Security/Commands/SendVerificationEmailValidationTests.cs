using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class SendVerificationEmailValidationTests : FluentValidationTests
    {
        [Theory, InlineData(null), InlineData(""), InlineData("\t  \r\n")]
        public void IsInvalid_WhenEmailAddress_IsEmpty(string emailAddress)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSendVerificationEmailCommand(queries.Object);
            var command = new SendVerificationEmail { EmailAddress = emailAddress };

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage
                .ShouldEqual(Resources.notempty_error.Replace("{PropertyName}", EmailAddress.Constraints.Label));
        }

        [Theory, InlineData("invalid"), InlineData("invalid@"), InlineData("invalid@gmail"), InlineData("invalid@gmail."), InlineData("invalid@.com")]
        public void IsInvalid_WhenEmailAddress_DoesNotMatchPattern(string emailAddress)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSendVerificationEmailCommand(queries.Object);
            var command = new SendVerificationEmail
            {
                EmailAddress = emailAddress,
            };

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources.email_error
                .Replace("{PropertyName}", EmailAddress.Constraints.Label)
                .Replace("{PropertyValue}", command.EmailAddress)
            );
        }

        [Fact]
        public void IsInvalid_WhenEmailAddressLength_IsGreaterThan_MaxLength()
        {
            var emailAddress = "a0@gmail.com";
            while (emailAddress.Length <= EmailAddress.Constraints.ValueMaxLength)
                emailAddress = emailAddress.Replace("0", "a0");
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSendVerificationEmailCommand(queries.Object);
            var command = new SendVerificationEmail { EmailAddress = emailAddress };

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources.Validation_MaxLength
                .Replace("{PropertyName}", EmailAddress.Constraints.Label)
                .Replace("{MaxLength}", EmailAddress.Constraints.ValueMaxLength.ToString(CultureInfo.InvariantCulture))
                .Replace("{TotalLength}", command.EmailAddress.Length.ToString(CultureInfo.InvariantCulture))
            );
        }

        [Fact]
        public void IsInvalid_WhenEmailAddress_IsAlreadyVerified()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSendVerificationEmailCommand(queries.Object);
            var command = new SendVerificationEmail { EmailAddress = "verified@domain.com" };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == command.EmailAddress;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(new EmailAddress
            {
                Value = command.EmailAddress,
                IsVerified = true,
            }));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources.Validation_EmailAddress_IsAlreadyVerified
                .Replace("{PropertyName}", EmailAddress.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", command.EmailAddress)
            );
        }

        [Fact]
        public void IsInvalid_WhenIsExpectingEmail_IsFalse()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSendVerificationEmailCommand(queries.Object);
            var command = new SendVerificationEmail();

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.IsExpectingEmail);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources.Validation_SendVerificationEmail_IsExpectingEmail
                .Replace("{PropertyName}", EmailAddress.Constraints.Label.ToLower())
            );
        }

        [Theory, InlineData(null), InlineData(EmailVerificationPurpose.Invalid)]
        public void IsInvalid_WhenPurpose_IsZero(EmailVerificationPurpose? purpose)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSendVerificationEmailCommand(queries.Object);
            var command = new SendVerificationEmail();
            if (purpose.HasValue) command.Purpose = purpose.Value;

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Purpose);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources.Validation_EmailVerificationPurpose_IsEmpty
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
        }

        [Theory, InlineData(EmailVerificationPurpose.CreateLocalUser),
            InlineData(EmailVerificationPurpose.CreateRemoteUser),
            InlineData(EmailVerificationPurpose.ForgotPassword)]
        public void IsValid_WhenAllRulesPass(EmailVerificationPurpose purpose)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateSendVerificationEmailCommand(queries.Object);
            var command = new SendVerificationEmail
            {
                EmailAddress = "valid@gmail.com",
                IsExpectingEmail = true,
                Purpose = purpose,
                SendFromUrl = "[send from this url]",
                VerifyUrlFormat = "[here is the token:' {0}']",
            };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == command.EmailAddress;
            User user = new ProxiedUser(new Random().Next(1, int.MaxValue));
            var emailAddress = new EmailAddress
            {
                Value = command.EmailAddress,
                User = user,
            };
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(emailAddress));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
        }
    }
}
