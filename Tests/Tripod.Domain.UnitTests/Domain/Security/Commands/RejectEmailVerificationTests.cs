using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class RejectEmailVerificationTests : FluentValidationTests
    {
        [Fact]
        public void Validator_TicketIsInvalid_WhenTokenIsNotValid()
        {
            var command = new RejectEmailVerification
            {
                Ticket = FakeData.String(),
                Token = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var emailVerification = new EmailVerification
            {
                Ticket = command.Ticket,
                Token = FakeData.String(),
                Purpose = EmailVerificationPurpose.AddEmail,
                ExpiresOnUtc = DateTime.UtcNow.AddMinutes(5),
            };
            Expression<Func<EmailVerificationBy, bool>> expectedEmailVerificationQuery =
                x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedEmailVerificationQuery)))
                .Returns(Task.FromResult(emailVerification));
            Expression<Func<EmailVerificationTokenIsValid, bool>> expectedTokenValidationQuery =
                x => x.Ticket == command.Ticket && x.Token == command.Token && x.Purpose == emailVerification.Purpose;
            queries.Setup(x => x.Execute(It.Is(expectedTokenValidationQuery)))
                .Returns(Task.FromResult(false));
            var validator = new ValidateRejectEmailVerificationCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_HasInvalidToken
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
        }

        [Fact]
        public void Handler_LoadsEmailVerification_ByTicket()
        {
            var command = new RejectEmailVerification
            {
                Ticket = FakeData.String(),
            };
            var emailVerification = new EmailVerification
            {
                Ticket = command.Ticket,
                EmailAddress = new EmailAddress(),
            };
            var emailVerificationData = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                emailVerification,
                new EmailVerification { Ticket = FakeData.String(), },
            };
            var emailVerificationSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict)
                .SetupDataAsync(emailVerificationData.AsQueryable());
            var firstTicket = FakeData.String();
            var secondTicket = FakeData.String();
            var ticketGenerationIndex = 0;
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<RandomSecret>()))
                .Returns(() => ticketGenerationIndex++ < 5 ? firstTicket : secondTicket);
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Get<EmailVerification>()).Returns(emailVerificationSet.Object);
            entities.Setup(x => x.Query<EmailVerification>()).Returns(new[]
            {
                new EmailVerification { Ticket = firstTicket, },
            }.AsQueryable);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleRejectEmailVerificationCommand(queries.Object, entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<EmailVerification>(), Times.Once);
        }
    }
}
