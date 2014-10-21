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
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class MustFindUserByVerifiedEmailTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustFindUserByVerifiedEmail(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void IsInvalid_WhenEmailAddress_IsEmpty(string emailAddress)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByVerifiedEmailCommand { EmailAddress = emailAddress };
            queries.Setup(x => x.Execute(It.IsAny<EmailAddressBy>())).Returns(Task.FromResult(null as EmailAddress));
            var validator = new FakeMustFindUserByVerifiedEmailValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> emailError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(emailError).ShouldEqual(1);
            result.Errors.Single(emailError).ErrorMessage.ShouldEqual(Resources.Validation_CouldNotFind
                .Replace("{PropertyName}", EmailAddress.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", emailAddress)
            );
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressBy>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenEmailAddress_IsNotFound()
        {
            var emailAddress = string.Format("{0}@domain.tld", Guid.NewGuid());
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByVerifiedEmailCommand { EmailAddress = emailAddress };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == emailAddress;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as EmailAddress));
            var validator = new FakeMustFindUserByVerifiedEmailValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> emailError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(emailError).ShouldEqual(1);
            result.Errors.Single(emailError).ErrorMessage.ShouldEqual(Resources.Validation_CouldNotFind
                .Replace("{PropertyName}", EmailAddress.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", emailAddress)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsInvalid_WhenEmailAddress_IsFound_ButHasNullUser()
        {
            var emailAddress = string.Format("{0}@domain.tld", Guid.NewGuid());
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var entity = new ProxiedEmailAddress(new Random().Next(1, int.MaxValue))
            {
                IsVerified = true,
                Value = emailAddress,
            };
            var command = new FakeMustFindUserByVerifiedEmailCommand { EmailAddress = emailAddress };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == emailAddress;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity as EmailAddress));
            var validator = new FakeMustFindUserByVerifiedEmailValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> emailError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(emailError).ShouldEqual(1);
            result.Errors.Single(emailError).ErrorMessage.ShouldEqual(Resources.Validation_CouldNotFind
                .Replace("{PropertyName}", EmailAddress.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", emailAddress)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenEmailAddress_IsFound_WithNonNullUser()
        {
            var emailAddress = string.Format("{0}@domain.tld", Guid.NewGuid());
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var user = new User();
            var entity = new ProxiedEmailAddress(new Random().Next(1, int.MaxValue))
            {
                IsVerified = true,
                Value = emailAddress,
                User = user,
            };
            var command = new FakeMustFindUserByVerifiedEmailCommand { EmailAddress = emailAddress };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == emailAddress;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity as EmailAddress));
            var validator = new FakeMustFindUserByVerifiedEmailValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.EmailAddress, command.EmailAddress);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustFindUserByVerifiedEmailCommand
    {
        public string EmailAddress { get; set; }
    }

    public class FakeMustFindUserByVerifiedEmailValidator : AbstractValidator<FakeMustFindUserByVerifiedEmailCommand>
    {
        public FakeMustFindUserByVerifiedEmailValidator(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddress)
                .MustFindUserByVerifiedEmail(queries)
                .WithName(EmailAddress.Constraints.Label)
            ;
        }
    }
}
