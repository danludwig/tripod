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
    public class MustNotBeRedeemedVerifyEmailTicketTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustNotBeRedeemedVerifyEmailTicket(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" \t ")]
        public void IsValid_WhenTicket_IsEmpty(string ticket)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBeRedeemedVerifyEmailTicketCommand { Ticket = ticket, };
            var validator = new FakeMustNotBeRedeemedVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.Ticket, command.Ticket);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
        }

        [Fact]
        public void IsValid_WhenEmailVerification_IsNotFoundByTicket()
        {
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBeRedeemedVerifyEmailTicketCommand { Ticket = ticket, };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as EmailVerification));
            var validator = new FakeMustNotBeRedeemedVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Ticket, command.Ticket);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenEmailVerification_IsNotAlreadyRedeemed()
        {
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBeRedeemedVerifyEmailTicketCommand { Ticket = ticket, };
            var entity = new EmailVerification { Ticket = ticket, };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity));
            var validator = new FakeMustNotBeRedeemedVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Ticket, command.Ticket);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsInvalid_WhenEmailVerification_IsAlreadyRedeemed()
        {
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBeRedeemedVerifyEmailTicketCommand { Ticket = ticket, };
            var entity = new EmailVerification
            {
                Ticket = ticket,
                RedeemedOnUtc = DateTime.UtcNow.AddMinutes(-1)
            };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity));
            var validator = new FakeMustNotBeRedeemedVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_IsRedeemed
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command.Ticket);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustNotBeRedeemedVerifyEmailTicketCommand
    {
        public string Ticket { get; set; }
    }

    public class FakeMustNotBeRedeemedVerifyEmailTicketValidator :
        AbstractValidator<FakeMustNotBeRedeemedVerifyEmailTicketCommand>
    {
        public FakeMustNotBeRedeemedVerifyEmailTicketValidator(IProcessQueries queries)
        {
            RuleFor(x => x.Ticket)
                .MustNotBeRedeemedVerifyEmailTicket(queries)
                .WithName(EmailVerification.Constraints.Label)
            ;
        }
    }
}
