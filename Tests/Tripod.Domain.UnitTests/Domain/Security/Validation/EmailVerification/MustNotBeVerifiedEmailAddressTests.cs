using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class MustNotBeVerifiedEmailAddressTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustNotBeVerifiedEmailAddress(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenEmailAddress_IsFound_ButIsVerified()
        {
            EmailAddress emailAddress = new ProxiedEmailAddress(FakeData.Id())
            {
                Value = FakeData.Email(),
                IsVerified = true,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBeVerifiedEmailAddressCommand { EmailAddress = emailAddress.Value };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == emailAddress.Value;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(emailAddress));
            var validator = new FakeMustNotBeVerifiedEmailAddressValidator(queries.Object);

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
            EmailAddress emailAddress = new ProxiedEmailAddress(FakeData.Id())
            {
                Value = FakeData.Email(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBeVerifiedEmailAddressCommand { EmailAddress = emailAddress.Value };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == emailAddress.Value;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(emailAddress));
            var validator = new FakeMustNotBeVerifiedEmailAddressValidator(queries.Object);

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
            var command = new FakeMustNotBeVerifiedEmailAddressCommand { EmailAddress = emailAddress };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == emailAddress;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as EmailAddress));
            var validator = new FakeMustNotBeVerifiedEmailAddressValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustNotBeVerifiedEmailAddressCommand
    {
        public string EmailAddress { get; set; }
    }

    public class FakeMustNotBeVerifiedEmailAddressValidator : AbstractValidator<FakeMustNotBeVerifiedEmailAddressCommand>
    {
        public FakeMustNotBeVerifiedEmailAddressValidator(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddress)
                .MustNotBeVerifiedEmailAddress(queries)
                .WithName(EmailAddress.Constraints.Label)
            ;
        }
    }
}
