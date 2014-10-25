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
    public class MustBeVerifiedEmailSecretTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () =>new MustBeVerifiedEmailSecret<object>(null, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenTicket_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustBeVerifiedEmailSecret<object>(queries.Object, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("ticket");
        }

        [Fact]
        public void IsInvalid_WhenEmailVerificationByTicket_HasSecretNotEqualToCommandSecret()
        {
            string secret = Guid.NewGuid().ToString().ToUpper();
            string ticket = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiedEmailSecretCommand { Secret = secret, Ticket = ticket, };
            var entity = new EmailVerification { Secret = secret.ToLower(), Ticket = ticket, };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity));
            var validator = new FakeMustBeVerifiedEmailSecretValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> secretError = x => x.PropertyName == command.PropertyName(y => y.Secret);
            result.Errors.Count(secretError).ShouldEqual(1);
            result.Errors.Single(secretError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationSecret_IsWrong
                .Replace("{PropertyName}", EmailVerification.Constraints.SecretLabel.ToLower())
                .Replace("{PropertyValue}", command.Secret)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Secret, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public void IsValid_WhenSecret_IsEmpty(string secret)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiedEmailSecretCommand
            {
                Secret = secret,
            };
            var validator = new FakeMustBeVerifiedEmailSecretValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.Secret, command);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
        }

        [Fact]
        public void IsValid_WhenEmailVerificationByTicket_IsNotFound()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiedEmailSecretCommand
            {
                Secret = Guid.NewGuid().ToString(),
                Ticket = Guid.NewGuid().ToString(),
            };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as EmailVerification));
            var validator = new FakeMustBeVerifiedEmailSecretValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Secret, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenEmailVerificationByTicket_HasSecretEqualingCommandSecret()
        {
            string secret = Guid.NewGuid().ToString();
            string ticket = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeVerifiedEmailSecretCommand { Secret = secret, Ticket = ticket, };
            var entity = new EmailVerification { Secret = secret, Ticket = ticket, };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity));
            var validator = new FakeMustBeVerifiedEmailSecretValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Secret, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustBeVerifiedEmailSecretCommand
    {
        public string Secret { get; set; }
        public string Ticket { get; set; }
    }

    public class FakeMustBeVerifiedEmailSecretValidator : AbstractValidator<FakeMustBeVerifiedEmailSecretCommand>
    {
        public FakeMustBeVerifiedEmailSecretValidator(IProcessQueries queries)
        {
            RuleFor(x => x.Secret)
                .MustBeVerifiedEmailSecret(queries, x => x.Ticket)
                .WithName(EmailVerification.Constraints.SecretLabel)
            ;
        }
    }
}
