using System;
using System.Diagnostics;
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
    public class MustBePurposedVerifyEmailTicketTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustBePurposedVerifyEmailTicket<object>(null, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void Ctor_ThrowsArgumentNullException_Purposes_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustBePurposedVerifyEmailTicket<object>(queries.Object, null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("purposes");
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.ForgotPassword,
            EmailVerificationPurpose.AddEmail, EmailVerificationPurpose.CreateRemoteUser)]
        [InlineData(EmailVerificationPurpose.CreateLocalUser,
            EmailVerificationPurpose.ForgotPassword, EmailVerificationPurpose.AddEmail)]
        [InlineData(EmailVerificationPurpose.AddEmail,
            EmailVerificationPurpose.ForgotPassword, null)]
        public void IsInvalid_WhenEmailVerificationByTicket_IsNotAllowedPurpose(
            EmailVerificationPurpose entityPurpose, EmailVerificationPurpose allowedPurpose1, EmailVerificationPurpose? allowedPurpose2)
        {
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBePurposedVerifyEmailTicketCommand
            {
                Ticket = ticket,
                Purpose1 = allowedPurpose1,
                Purpose2 = allowedPurpose2,
            };
            var entity = new EmailVerification { Ticket = ticket, Purpose = entityPurpose, };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(entity));
            var validator = new FakeMustBePurposedVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_IsWrongPurpose
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" \t ")]
        public void IsValid_WhenTicket_IsNullOrWhitespace(string ticket)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBePurposedVerifyEmailTicketCommand
            {
                Ticket = ticket,
            };
            var validator = new FakeMustBePurposedVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.Ticket, command);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
        }

        [Fact]
        public void IsValid_WhenEmailVerificationByTicket_DoesNotExist()
        {
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBePurposedVerifyEmailTicketCommand
            {
                Ticket = ticket,
            };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(null as EmailVerification));
            var validator = new FakeMustBePurposedVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Ticket, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.ForgotPassword,
            EmailVerificationPurpose.AddEmail, EmailVerificationPurpose.ForgotPassword)]
        [InlineData(EmailVerificationPurpose.CreateLocalUser,
            EmailVerificationPurpose.CreateLocalUser, EmailVerificationPurpose.CreateRemoteUser)]
        [InlineData(EmailVerificationPurpose.AddEmail,
            EmailVerificationPurpose.AddEmail, null)]
        public void IsValid_WhenEmailVerificationByTicket_IsAllowedPurpose(
            EmailVerificationPurpose entityPurpose, EmailVerificationPurpose allowedPurpose1, EmailVerificationPurpose? allowedPurpose2)
        {
            var ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustBePurposedVerifyEmailTicketCommand
            {
                Ticket = ticket,
                Purpose1 = allowedPurpose1,
                Purpose2 = allowedPurpose2,
            };
            var entity = new EmailVerification { Ticket = ticket, Purpose = entityPurpose, };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(entity));
            var validator = new FakeMustBePurposedVerifyEmailTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Ticket, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustBePurposedVerifyEmailTicketCommand
    {
        public string Ticket { get; [UsedImplicitly] set; }
        public EmailVerificationPurpose Purpose1 { get; set; }
        public EmailVerificationPurpose? Purpose2 { get; set; }
    }

    public class FakeMustBePurposedVerifyEmailTicketValidator :
        AbstractValidator<FakeMustBePurposedVerifyEmailTicketCommand>
    {
        public FakeMustBePurposedVerifyEmailTicketValidator(IProcessQueries queries)
        {
            When(x => !x.Purpose2.HasValue, () =>
                RuleFor(x => x.Ticket)
                    .MustBePurposedVerifyEmailTicket(queries, x => x.Purpose1)
                    .WithName(EmailVerification.Constraints.Label)
            );

            When(x => x.Purpose2.HasValue, () =>
                RuleFor(x => x.Ticket)
                    .MustBePurposedVerifyEmailTicket(queries, x => x.Purpose1, x =>
                    {
                        Debug.Assert(x.Purpose2.HasValue);
                        return x.Purpose2.Value;
                    })
                    .WithName(EmailVerification.Constraints.Label)
            );
        }
    }
}
