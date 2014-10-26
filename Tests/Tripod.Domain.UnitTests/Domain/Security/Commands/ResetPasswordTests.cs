using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class ResetPasswordTests : FluentValidationTests
    {
        [Theory]
        [InlineData(null), InlineData(""), InlineData(" ")]
        public void Validator_ConfirmPassword_IsInvalid_WhenEmpty(string confirmPassword)
        {
            var command = new ResetPassword { Password = confirmPassword, };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateResetPasswordCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.ConfirmPassword);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .notempty_error
                .Replace("{PropertyName}", LocalMembership.Constraints.PasswordConfirmationLabel)
            );
        }

        [Fact]
        public void Validator_ConfirmPassword_IsInvalid_WhenNotEqualToPassword()
        {
            var command = new ResetPassword
            {
                Password = FakeData.String(),
                ConfirmPassword = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateResetPasswordCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.ConfirmPassword);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_PasswordDoesNotEqualConfirmation
                .Replace("{PropertyName}", LocalMembership.Constraints.PasswordConfirmationLabel)
                .Replace("{PasswordLabel}", LocalMembership.Constraints.PasswordLabel)
            );
        }

        [Fact]
        public void Validator_Ticket_IsInvalid_WhenTokenIsInvalid()
        {
            var command = new ResetPassword
            {
                Ticket = FakeData.String(),
                Token = FakeData.String(),
            };
            var emailVerification = new EmailVerification
            {
                Ticket = command.Ticket,
                ExpiresOnUtc = DateTime.UtcNow.AddMinutes(5),
                Purpose = EmailVerificationPurpose.ForgotPassword,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<EmailVerificationBy, bool>> expectedEmailVerificationQuery =
                x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedEmailVerificationQuery)))
                .Returns(Task.FromResult(emailVerification));
            Expression<Func<EmailVerificationTokenIsValid, bool>> expectedTokenValidationQuery =
                x => x.Ticket == command.Ticket && x.Token == command.Token
                    && x.Purpose == emailVerification.Purpose;
            queries.Setup(x => x.Execute(It.Is(expectedTokenValidationQuery)))
                .Returns(Task.FromResult(false));
            var validator = new ValidateResetPasswordCommand(queries.Object);

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
            var command = new ResetPassword
            {
                Ticket = FakeData.String(),
                Password = FakeData.String(),
            };
            var emailAddress = new EmailAddress
            {
                Value = FakeData.Email(),
                User = new User(),
            };
            var emailVerification = new EmailVerification
            {
                Ticket = command.Ticket,
                EmailAddress = emailAddress,
            };
            var data = new[]
            {
                new EmailVerification { Ticket = FakeData.String(), },
                emailVerification,
                new EmailVerification { Ticket = FakeData.String(), },
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<LocalMembershipByVerifiedEmail, bool>> expectedLocalMembershipQuery =
                x => x.EmailAddress == emailAddress.Value;
            queries.Setup(x => x.Execute(It.Is(expectedLocalMembershipQuery)))
                .Returns(Task.FromResult(null as LocalMembership));
            Expression<Func<HashedPassword, bool>> expectedHashedPasswordQuery =
                x => x.Password == command.Password;
            queries.Setup(x => x.Execute(It.Is(expectedHashedPasswordQuery)))
                .Returns(Task.FromResult(FakeData.String()));
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var dbSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict)
                .SetupDataAsync(data.AsQueryable());
            entities.Setup(x => x.Get<EmailVerification>()).Returns(dbSet.Object);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleResetPasswordCommand(queries.Object, entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<EmailVerification>(), Times.Once);
        }
    }
}
