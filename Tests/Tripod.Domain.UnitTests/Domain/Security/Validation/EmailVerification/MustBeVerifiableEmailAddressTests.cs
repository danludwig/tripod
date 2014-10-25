using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class MustBeVerifiableEmailAddressTests : FluentValidationTests
    {
        [Fact]
        public void ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new FakeMustBeVerifiableEmailAddressValidator(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" \t ")]
        public void IsInvalid_WhenEmailAddress_IsEmpty(string emailAddress)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiableEmailAddressCommand { EmailAddress = emailAddress, };
            var validator = new FakeMustBeVerifiableEmailAddressValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> emailError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(emailError).ShouldEqual(1);
            result.Errors.Single(emailError).ErrorMessage.ShouldEqual(Resources
                .notempty_error
                .Replace("{PropertyName}", EmailAddress.Constraints.Label)
            );
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressBy>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressBy>()), Times.Never);
        }

        [Theory]
        [InlineData("user@domain..tld")]
        [InlineData("user@@domain.tld")]
        [InlineData("user@domaintld")]
        public void IsInvalid_WhenEmailAddress_DoesNotMatchEmailRegularExpression(string emailAddress)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiableEmailAddressCommand { EmailAddress = emailAddress, };
            var validator = new FakeMustBeVerifiableEmailAddressValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> emailError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(emailError).ShouldEqual(1);
            result.Errors.Single(emailError).ErrorMessage.ShouldEqual(Resources
                .email_error
                .Replace("{PropertyValue}", emailAddress)
            );
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressBy>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenEmailAddress_IsGreaterThanMaxLength()
        {
            var emailAddress = FakeData.Email();
            while (emailAddress.Length < EmailAddress.Constraints.ValueMaxLength)
                emailAddress = Guid.NewGuid() + emailAddress;
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiableEmailAddressCommand { EmailAddress = emailAddress, };
            var validator = new FakeMustBeVerifiableEmailAddressValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> emailError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(emailError).ShouldEqual(1);
            result.Errors.Single(emailError).ErrorMessage.ShouldEqual(Resources
                .Validation_MaxLength
                .Replace("{PropertyName}", EmailAddress.Constraints.Label)
                .Replace("{MaxLength}", EmailAddress.Constraints.ValueMaxLength
                    .ToString(CultureInfo.InvariantCulture))
                .Replace("{TotalLength}", emailAddress.Length.ToString(CultureInfo.InvariantCulture))
            );
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressBy>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenEmailAddress_IsFound_ButIsVerified()
        {
            EmailAddress emailAddress = new ProxiedEmailAddress(new Random().Next(1, int.MaxValue))
            {
                Value = FakeData.Email(),
                IsVerified = true,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiableEmailAddressCommand { EmailAddress = emailAddress.Value };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == emailAddress.Value;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(emailAddress));
            var validator = new FakeMustBeVerifiableEmailAddressValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> emailError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(emailError).ShouldEqual(1);
            result.Errors.Single(emailError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailAddress_IsAlreadyVerified
                .Replace("{PropertyName}", EmailAddress.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", command.EmailAddress)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenEmailAddress_IsFound_AndIsNotVerified()
        {
            EmailAddress emailAddress = new ProxiedEmailAddress(new Random().Next(1, int.MaxValue))
            {
                Value = FakeData.Email(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiableEmailAddressCommand { EmailAddress = emailAddress.Value };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == emailAddress.Value;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(emailAddress));
            var validator = new FakeMustBeVerifiableEmailAddressValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenEmailAddress_IsNotFound()
        {
            var emailAddress = FakeData.Email();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiableEmailAddressCommand { EmailAddress = emailAddress };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == emailAddress;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as EmailAddress));
            var validator = new FakeMustBeVerifiableEmailAddressValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustBeVerifiableEmailAddressCommand
    {
        public string EmailAddress { get; set; }
    }

    public class FakeMustBeVerifiableEmailAddressValidator : AbstractValidator<FakeMustBeVerifiableEmailAddressCommand>
    {
        public FakeMustBeVerifiableEmailAddressValidator(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddress)
                .MustBeVerifiableEmailAddress(queries)
                .WithName(EmailAddress.Constraints.Label)
            ;
        }
    }
}
