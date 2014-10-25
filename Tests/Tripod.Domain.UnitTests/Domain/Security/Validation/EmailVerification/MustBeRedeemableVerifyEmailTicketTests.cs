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
    public class MustBeRedeemableVerifyEmailTicketTests : FluentValidationTests
    {
        [Fact]
        public void ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new FakeMustBeRedeemableVerifyEmailTicketValidator(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" \t ")]
        public void IsInvalid_WhenTicket_IsEmpty(string ticket)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeRedeemableVerifyEmailTicketCommand { Ticket = ticket, };
            var validator = new FakeMustBeRedeemableVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .notempty_error
                .Replace("{PropertyName}", EmailVerification.Constraints.Label)
            );
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command.Ticket);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenEmailVerification_IsNotFoundByTicket()
        {
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeRedeemableVerifyEmailTicketCommand { Ticket = ticket, };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as EmailVerification));
            var validator = new FakeMustBeRedeemableVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .Validation_DoesNotExist_NoValue
                .Replace("{PropertyName}", EmailVerification.Constraints.Label)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command.Ticket);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsInvalid_WhenEmailVerification_IsAlreadyRedeemed()
        {
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeRedeemableVerifyEmailTicketCommand { Ticket = ticket, };
            var entity = new EmailVerification
            {
                Ticket = ticket,
                RedeemedOnUtc = DateTime.UtcNow.AddMinutes(-1),
            };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity));
            var validator = new FakeMustBeRedeemableVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_IsRedeemed
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command.Ticket);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(4));
        }

        [Fact]
        public void IsInvalid_WhenEmailVerification_HasExpired()
        {
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeRedeemableVerifyEmailTicketCommand { Ticket = ticket, };
            var entity = new EmailVerification
            {
                Ticket = ticket,
                ExpiresOnUtc = DateTime.UtcNow.AddMinutes(-1),
            };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity));
            var validator = new FakeMustBeRedeemableVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_IsExpired
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(3));
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command.Ticket);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(6));
        }

        [Fact]
        public void IsValid_WhenEmailVerification_IsNotEmpty_AndIsFoundByTicket_AndIsNotRedeemed_AndIsNotExpired()
        {
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBeRedeemableVerifyEmailTicketCommand { Ticket = ticket, };
            var entity = new EmailVerification
            {
                Ticket = ticket,
                ExpiresOnUtc = DateTime.UtcNow.AddMinutes(1),
            };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity));
            var validator = new FakeMustBeRedeemableVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(3));
            validator.ShouldNotHaveValidationErrorFor(x => x.Ticket, command.Ticket);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(6));
        }
    }

    public class FakeMustBeRedeemableVerifyEmailTicketCommand
    {
        public string Ticket { get; set; }
    }

    public class FakeMustBeRedeemableVerifyEmailTicketValidator : AbstractValidator<FakeMustBeRedeemableVerifyEmailTicketCommand>
    {
        public FakeMustBeRedeemableVerifyEmailTicketValidator(IProcessQueries queries)
        {
            RuleFor(x => x.Ticket)
                .MustBeRedeemableVerifyEmailTicket(queries)
                .WithName(EmailVerification.Constraints.Label)
            ;
        }
    }
}
