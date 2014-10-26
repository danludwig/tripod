using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class VerifyEmailSecretTests : FluentValidationTests
    {
        [Theory]
        [InlineData(null), InlineData(""), InlineData(" ")]
        public void Validator_SecretIsInvalid_WhenEmpty(string secret)
        {
            var command = new VerifyEmailSecret { Secret = secret };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateVerifyEmailSecretCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Secret);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .notempty_error
                .Replace("{PropertyName}", EmailVerification.Constraints.SecretLabel)
            );
        }

        [Fact]
        public void Validator_SecretIsInvalid_WhenDoesNotMatchTicketSecret()
        {
            var command = new VerifyEmailSecret
            {
                Secret = FakeData.String(),
                Ticket = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.Is<EmailVerificationBy>(y => y.Ticket == command.Ticket)))
                .Returns(Task.FromResult(new EmailVerification
                {
                    Ticket = command.Ticket,
                    Secret = FakeData.String(),
                }));
            var validator = new ValidateVerifyEmailSecretCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Secret);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationSecret_IsWrong
                .Replace("{PropertyName}", EmailVerification.Constraints.SecretLabel.ToLower())
                .Replace("{PropertyValue}", command.Secret)
            );
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.AddEmail, EmailVerificationPurpose.CreateRemoteUser)]
        [InlineData(EmailVerificationPurpose.CreateLocalUser, EmailVerificationPurpose.ForgotPassword)]
        public void Validator_TicketIsInvalid_WhenDoesNotMatchCommandPurpose(
            EmailVerificationPurpose commandPurpose, EmailVerificationPurpose entityPurpose)
        {
            var command = new VerifyEmailSecret
            {
                Ticket = FakeData.String(),
                Purpose = commandPurpose,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.Is<EmailVerificationBy>(y => y.Ticket == command.Ticket)))
                .Returns(Task.FromResult(new EmailVerification
                {
                    Ticket = command.Ticket,
                    ExpiresOnUtc = DateTime.UtcNow.AddMinutes(5),
                    Purpose = entityPurpose,
                }));
            var validator = new ValidateVerifyEmailSecretCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_IsWrongPurpose
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", command.Secret)
            );
        }

        [Fact]
        public void Handler_QueriesEmailVerification_ByTicket()
        {
            var command = new VerifyEmailSecret
            {
                Ticket = FakeData.String(),
            };
            var entities = new Mock<IWriteEntities>();
            var data = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                new EmailVerification { Ticket = command.Ticket, },
                new EmailVerification { Ticket = FakeData.String(), },
            };
            var dbSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            entities.Setup(x => x.Query<EmailVerification>()).Returns(dbSet.Object);
            var handler = new HandleVerifyEmailSecretCommand(entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Query<EmailVerification>(), Times.Once);
        }
    }
}
