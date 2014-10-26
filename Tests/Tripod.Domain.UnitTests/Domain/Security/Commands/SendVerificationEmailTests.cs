using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class SendVerificationEmailTests : FluentValidationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t  \r\n")]
        public void Validator_IsInvalid_WhenEmailAddress_IsEmpty(string emailAddress)
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
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("invalid@")]
        [InlineData("invalid@gmail")]
        [InlineData("invalid@gmail.")]
        [InlineData("invalid@.com")]
        public void Validator_IsInvalid_WhenEmailAddress_DoesNotMatchPattern(string emailAddress)
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
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command);
        }

        [Fact]
        public void Validator_IsInvalid_WhenEmailAddressLength_IsGreaterThan_MaxLength()
        {
            var emailAddress = FakeData.Email();
            while (emailAddress.Length <= EmailAddress.Constraints.ValueMaxLength)
                emailAddress = FakeData.String() + emailAddress;
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
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command);
        }

        [Fact]
        public void Validator_IsInvalid_WhenEmailAddress_IsAlreadyVerified()
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
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command);
        }

        [Fact]
        public void Validator_IsInvalid_WhenIsExpectingEmail_IsFalse()
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
            validator.ShouldHaveValidationErrorFor(x => x.IsExpectingEmail, command);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(EmailVerificationPurpose.Invalid)]
        public void Validator_IsInvalid_WhenPurpose_IsZero(EmailVerificationPurpose? purpose)
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
            validator.ShouldHaveValidationErrorFor(x => x.Purpose, command);
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.CreateLocalUser)]
        [InlineData(EmailVerificationPurpose.CreateRemoteUser)]
        [InlineData(EmailVerificationPurpose.ForgotPassword)]
        public void Validator_IsValid_WhenAllRulesPass(EmailVerificationPurpose purpose)
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
            User user = new ProxiedUser(FakeData.Id());
            var emailAddress = new EmailAddress
            {
                Value = command.EmailAddress,
                User = user,
            };
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(emailAddress));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.Purpose, command);
        }

        [Fact]
        public void Handler_CreatesEmailVerification()
        {
            var command = new SendVerificationEmail
            {
                EmailAddress = FakeData.Email(),
                Purpose = EmailVerificationPurpose.ForgotPassword,
            };
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            var emailVerification = new EmailVerification
            {
                Ticket = FakeData.String(),
                Token = FakeData.String(),
                EmailAddress = new EmailAddress { Value = command.EmailAddress, },
                Purpose = EmailVerificationPurpose.ForgotPassword,
                Secret = FakeData.String(),
            };
            Expression<Func<CreateEmailVerification, bool>> expectedSubCommand = x =>
                x.Commit == false && x.EmailAddress == command.EmailAddress &&
                x.Purpose == command.Purpose;
            commands.Setup(x => x.Execute(It.Is(expectedSubCommand)))
                .Callback<IDefineCommand>(x => ((CreateEmailVerification) x).CreatedEntity = emailVerification)
                .Returns(Task.FromResult(0));
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Create(It.IsAny<EmailMessage>()));
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var mail = new Mock<IDeliverEmailMessage>(MockBehavior.Strict);
            mail.Setup(x => x.Deliver(It.IsAny<int>()));
            var config = new Mock<IReadConfiguration>(MockBehavior.Strict);
            config.SetupGet(x => x.AppSettings).Returns(new NameValueCollection
            {
                { AppSettingKey.MailFromDefault.ToString(), FakeData.Email() },
            });
            var handler = new HandleSendVerificationEmailCommand(commands.Object,
                entities.Object, mail.Object, new AppConfiguration(config.Object));

            handler.Handle(command).Wait();

            commands.Verify(x => x.Execute(It.Is(expectedSubCommand)), Times.Once);
        }

        [Fact]
        public void Handler_SetsVerificationEmailUserId_ToPrincipalIdentityId_WhenPurposeIsAddEmail()
        {
            var command = new SendVerificationEmail
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.Id().ToString(CultureInfo.InvariantCulture)), 
                }, "authenticationType"), null),
                EmailAddress = FakeData.Email(),
                Purpose = EmailVerificationPurpose.AddEmail,
                VerifyUrlFormat = "https://sub.domain.tld/path/to",
                SendFromUrl = "https://sub.domain.tld/path/from",
            };
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            var emailVerification = new EmailVerification
            {
                Ticket = FakeData.String(),
                Token = FakeData.String(),
                EmailAddress = new EmailAddress { Value = command.EmailAddress, },
                Purpose = EmailVerificationPurpose.ForgotPassword,
                Secret = FakeData.String(),
            };
            Expression<Func<CreateEmailVerification, bool>> expectedSubCommand = x =>
                x.Commit == false && x.EmailAddress == command.EmailAddress &&
                x.Purpose == command.Purpose;
            commands.Setup(x => x.Execute(It.Is(expectedSubCommand)))
                .Callback<IDefineCommand>(x => ((CreateEmailVerification)x).CreatedEntity = emailVerification)
                .Returns(Task.FromResult(0));
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Create(It.IsAny<EmailMessage>()));
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var mail = new Mock<IDeliverEmailMessage>(MockBehavior.Strict);
            mail.Setup(x => x.Deliver(It.IsAny<int>()));
            var config = new Mock<IReadConfiguration>(MockBehavior.Strict);
            config.SetupGet(x => x.AppSettings).Returns(new NameValueCollection
            {
                { AppSettingKey.MailFromDefault.ToString(), FakeData.Email() },
            });
            var handler = new HandleSendVerificationEmailCommand(commands.Object,
                entities.Object, mail.Object, new AppConfiguration(config.Object));

            emailVerification.EmailAddress.UserId.ShouldBeNull();
            handler.Handle(command).Wait();

            commands.Verify(x => x.Execute(It.Is(expectedSubCommand)), Times.Once);
            emailVerification.EmailAddress.UserId.ShouldEqual(command.Principal.Identity.GetUserId<int>());
        }
    }
}
