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
    public class MustNotBePrimaryEmailAddressTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustNotBePrimaryEmailAddress(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenEmailAddress_IsFound_ButIsPrimary()
        {
            var emailAddress = new ProxiedEmailAddress(new Random().Next(1, int.MaxValue))
            {
                Value = string.Format("{0}@domain.tld", Guid.NewGuid()),
                IsPrimary = true,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBePrimaryEmailAddressCommand { EmailAddressId = emailAddress.Id };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Id == emailAddress.Id;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(emailAddress as EmailAddress));
            var validator = new FakeMustNotBePrimaryEmailAddressValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> emailError = x => x.PropertyName == command.PropertyName(y => y.EmailAddressId);
            result.Errors.Count(emailError).ShouldEqual(1);
            result.Errors.Single(emailError).ErrorMessage.ShouldEqual(Resources.Validation_EmailAddress_CannotBePrimary
                .Replace("{PropertyName}", EmailAddress.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddressId, command.EmailAddressId);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenEmailAddress_IsFound_AndIsNotPrimary()
        {
            var emailAddress = new ProxiedEmailAddress(new Random().Next(1, int.MaxValue))
            {
                Value = string.Format("{0}@domain.tld", Guid.NewGuid()),
                IsPrimary = false,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBePrimaryEmailAddressCommand { EmailAddressId = emailAddress.Id };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Id == emailAddress.Id;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(emailAddress as EmailAddress));
            var validator = new FakeMustNotBePrimaryEmailAddressValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.EmailAddressId, command.EmailAddressId);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenEmailAddress_IsNotFound()
        {
            var emailAddressId = new Random().Next(1, int.MaxValue);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBePrimaryEmailAddressCommand { EmailAddressId = emailAddressId };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Id == emailAddressId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as EmailAddress));
            var validator = new FakeMustNotBePrimaryEmailAddressValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.EmailAddressId, command.EmailAddressId);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustNotBePrimaryEmailAddressCommand
    {
        public int EmailAddressId { get; set; }
    }

    public class FakeMustNotBePrimaryEmailAddressValidator : AbstractValidator<FakeMustNotBePrimaryEmailAddressCommand>
    {
        public FakeMustNotBePrimaryEmailAddressValidator(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddressId)
                .MustNotBePrimaryEmailAddress(queries)
                .WithName(EmailAddress.Constraints.Label)
            ;
        }
    }
}
