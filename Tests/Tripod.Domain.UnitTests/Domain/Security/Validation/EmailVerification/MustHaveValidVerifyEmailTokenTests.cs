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
    public class MustHaveValidVerifyEmailTokenTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () =>new MustHaveValidVerifyEmailToken<object>(null, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenToken_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustHaveValidVerifyEmailToken<object>(queries.Object, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("token");
        }

        [Fact]
        public void IsInvalid_WhenEmailVerificationTokenIsValid_ReturnsFalse()
        {
            string token = FakeData.String();
            string ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustHaveValidVerifyEmailTokenCommand
            {
                Ticket = ticket,
                Token = token,
            };
            var entity = new EmailVerification
            {
                Ticket = ticket,
                Token = token,
                Purpose = EmailVerificationPurpose.ForgotPassword,
            };
            Expression<Func<EmailVerificationBy, bool>> ticketQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(ticketQuery))).Returns(Task.FromResult(entity));
            Expression<Func<EmailVerificationTokenIsValid, bool>> verifyQuery =
                x => x.Ticket == command.Ticket && x.Token == token && x.Purpose == entity.Purpose;
            queries.Setup(x => x.Execute(It.Is(verifyQuery))).Returns(Task.FromResult(false));
            var validator = new FakeMustHaveValidVerifyEmailTokenValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_HasInvalidToken
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.Is(ticketQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(verifyQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command);
            queries.Verify(x => x.Execute(It.Is(ticketQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.Is(verifyQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsInvalid_WhenCommandToken_DoesNotEqualEntityToken()
        {
            string token = FakeData.String().ToUpper();
            string ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustHaveValidVerifyEmailTokenCommand
            {
                Ticket = ticket,
                Token = token,
            };
            var entity = new EmailVerification
            {
                Ticket = ticket,
                Token = token.ToLower(),
                Purpose = EmailVerificationPurpose.ForgotPassword,
            };
            Expression<Func<EmailVerificationBy, bool>> ticketQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(ticketQuery))).Returns(Task.FromResult(entity));
            Expression<Func<EmailVerificationTokenIsValid, bool>> verifyQuery =
                x => x.Ticket == command.Ticket && x.Token == token && x.Purpose == entity.Purpose;
            queries.Setup(x => x.Execute(It.Is(verifyQuery))).Returns(Task.FromResult(true));
            var validator = new FakeMustHaveValidVerifyEmailTokenValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_HasInvalidToken
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.Is(ticketQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(verifyQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command);
            queries.Verify(x => x.Execute(It.Is(ticketQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.Is(verifyQuery)), Times.Exactly(2));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public void IsInvalid_WhenCommandToken_IsEmpty(string token)
        {
            string ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustHaveValidVerifyEmailTokenCommand
            {
                Ticket = ticket,
                Token = token,
            };
            var entity = new EmailVerification
            {
                Ticket = ticket,
                Token = token,
                Purpose = EmailVerificationPurpose.ForgotPassword,
            };
            Expression<Func<EmailVerificationBy, bool>> ticketQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(ticketQuery))).Returns(Task.FromResult(entity));
            Expression<Func<EmailVerificationTokenIsValid, bool>> verifyQuery =
                x => x.Ticket == command.Ticket && x.Token == token && x.Purpose == entity.Purpose;
            queries.Setup(x => x.Execute(It.Is(verifyQuery))).Returns(Task.FromResult(true));
            var validator = new FakeMustHaveValidVerifyEmailTokenValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_HasInvalidToken
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.Is(ticketQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(verifyQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command);
            queries.Verify(x => x.Execute(It.Is(ticketQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.Is(verifyQuery)), Times.Exactly(2));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public void IsValid_WhenTicket_IsEmpty(string ticket)
        {
            var token = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustHaveValidVerifyEmailTokenCommand
            {
                Ticket = ticket,
                Token = token,
            };
            var validator = new FakeMustHaveValidVerifyEmailTokenValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.Ticket, command);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
        }

        [Fact]
        public void IsValid_WhenEmailVerificationByTicket_IsNotFound()
        {
            var token = FakeData.String();
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustHaveValidVerifyEmailTokenCommand
            {
                Ticket = ticket,
                Token = token,
            };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as EmailVerification));
            var validator = new FakeMustHaveValidVerifyEmailTokenValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Ticket, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenEmailVerificationByTicketIsFound_AndTokenIsValid_AndTokensAreEqual_AndTokenIsNotEmpty()
        {
            string token = FakeData.String();
            string ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustHaveValidVerifyEmailTokenCommand
            {
                Ticket = ticket,
                Token = token,
            };
            var entity = new EmailVerification
            {
                Ticket = ticket,
                Token = token,
                Purpose = EmailVerificationPurpose.ForgotPassword,
            };
            Expression<Func<EmailVerificationBy, bool>> ticketQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(ticketQuery))).Returns(Task.FromResult(entity));
            Expression<Func<EmailVerificationTokenIsValid, bool>> verifyQuery =
                x => x.Ticket == command.Ticket && x.Token == token && x.Purpose == entity.Purpose;
            queries.Setup(x => x.Execute(It.Is(verifyQuery))).Returns(Task.FromResult(true));
            var validator = new FakeMustHaveValidVerifyEmailTokenValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(ticketQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(verifyQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Ticket, command);
            queries.Verify(x => x.Execute(It.Is(ticketQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.Is(verifyQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustHaveValidVerifyEmailTokenCommand
    {
        public string Ticket { get; set; }
        public string Token { get; set; }
    }

    public class FakeMustHaveValidVerifyEmailTokenValidator : AbstractValidator<FakeMustHaveValidVerifyEmailTokenCommand>
    {
        public FakeMustHaveValidVerifyEmailTokenValidator(IProcessQueries queries)
        {
            RuleFor(x => x.Ticket)
                .MustHaveValidVerifyEmailToken(queries, x => x.Token)
                .WithName(EmailVerification.Constraints.Label)
            ;
        }
    }
}
