using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class CreateLocalMembershipTests : FluentValidationTests
    {
        [Fact]
        public void Validator_IsInvalid_WhenUserByPrincipal_DoesNotExist()
        {
            var command = new CreateLocalMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity(FakeData.String(), "authenticationType"), null),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(null as User));
            var validator = new ValidateCreateLocalMembershipCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_DoesNotExist
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", command.Principal.Identity.Name)
            );
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command);
        }

        [Fact]
        public void Validator_IsInvalid_WhenUserName_IsUnverifiedEmail()
        {
            var command = new CreateLocalMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity("", ""), null),
                Ticket = FakeData.String(),
                UserName = FakeData.Email(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(null as User));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationBy>()))
                .Returns(Task.FromResult(new EmailVerification
                {
                    EmailAddress = new EmailAddress
                    {
                        Value = FakeData.Email(),
                    },
                }));
            var validator = new ValidateCreateLocalMembershipCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_UserName_AllowedEmailAddress
                .Replace("{PropertyName}", User.Constraints.NameLabel.ToLower())
                .Replace("{PropertyValue}", command.UserName)
            );
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command);
        }

        [Fact]
        public void Validator_IsInvalid_WhenConfirmPassword_DoesNotEqualPassword()
        {
            var command = new CreateLocalMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity("", ""), null),
                Ticket = FakeData.String(),
                UserName = FakeData.Email(),
                Password = FakeData.String(),
                ConfirmPassword = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(null as User));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationBy>()))
                .Returns(Task.FromResult(null as EmailVerification));
            var validator = new ValidateCreateLocalMembershipCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.ConfirmPassword);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_PasswordDoesNotEqualConfirmation
                .Replace("{PropertyName}", LocalMembership.Constraints.PasswordConfirmationLabel)
                .Replace("{PasswordLabel}", LocalMembership.Constraints.PasswordLabel.ToLower())
            );
            validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, command);
        }

        [Fact]
        public void Validator_IsInvalid_WhenTicketPurpose_IsNotCreateLocalUser()
        {
            var command = new CreateLocalMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity("", ""), null),
                Ticket = FakeData.String(),
                UserName = FakeData.String(),
                Password = FakeData.String(),
                ConfirmPassword = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(null as User));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationBy>()))
                .Returns(Task.FromResult(new EmailVerification
                {
                    Ticket = command.Ticket,
                    Purpose = FakeData.OneOf(EmailVerificationPurpose.AddEmail, EmailVerificationPurpose.CreateRemoteUser,
                        EmailVerificationPurpose.ForgotPassword, EmailVerificationPurpose.Invalid),
                    ExpiresOnUtc = DateTime.UtcNow.AddMinutes(5),
                }));
            var validator = new ValidateCreateLocalMembershipCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_IsWrongPurpose
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command);
        }

        [Fact]
        public void Validator_IsInvalid_WhenTicket_IsNotValidForToken()
        {
            var command = new CreateLocalMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity("", ""), null),
                Ticket = FakeData.String(),
                Token = FakeData.String(),
                UserName = FakeData.String(),
                Password = FakeData.String(),
                ConfirmPassword = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(null as User));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationBy>()))
                .Returns(Task.FromResult(new EmailVerification
                {
                    Ticket = command.Ticket,
                    Purpose = EmailVerificationPurpose.CreateLocalUser,
                    ExpiresOnUtc = DateTime.UtcNow.AddMinutes(5),
                }));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationTokenIsValid>()))
                .Returns(Task.FromResult(false));
            var validator = new ValidateCreateLocalMembershipCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailVerificationTicket_HasInvalidToken
                .Replace("{PropertyName}", EmailVerification.Constraints.Label.ToLower())
            );
            validator.ShouldHaveValidationErrorFor(x => x.Ticket, command);
        }

        [Fact]
        public void Handler_CreatesUser_WhenCommandPrincipalIsNull()
        {
            var command = new CreateLocalMembership
            {
                UserName = FakeData.String(),
                Ticket = FakeData.String(),
                Token = FakeData.String(),
                Password = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.Is<HashedPassword>(y => y.Password == command.Password)))
                .Returns(Task.FromResult(FakeData.String()));
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            Expression<Func<CreateUser, bool>> expectedCreateUserCommand = x => x.Name == command.UserName;
            commands.Setup(x => x.Execute(It.Is(expectedCreateUserCommand)))
                .Callback<IDefineCommand>(x => ((CreateUser)x).CreatedEntity = new User())
                .Returns(Task.FromResult(0));
            Expression<Func<RedeemEmailVerification, bool>> expectedRedeemEmailVerificationCommand =
                x => x.Commit == false && x.Token == command.Token && x.Ticket == command.Ticket;
            commands.Setup(x => x.Execute(It.Is(expectedRedeemEmailVerificationCommand)))
                .Returns(Task.FromResult(0));
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleCreateLocalMembershipCommand(
                queries.Object, commands.Object, entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.GetAsync<User>(It.IsAny<int>()), Times.Never);
            commands.Verify(x => x.Execute(It.Is(expectedCreateUserCommand)), Times.Once);
            commands.Verify(x => x.Execute(It.Is(expectedRedeemEmailVerificationCommand)), Times.Once);
        }

        [Fact]
        public void Handler_DoesNotCreateUser_WhenCommandPrincipal_HasAppUserId()
        {
            var command = new CreateLocalMembership
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.Id().ToString(CultureInfo.InvariantCulture)), 
                }, "authenticationType"), null),
                UserName = FakeData.String(),
                Ticket = FakeData.String(),
                Token = FakeData.String(),
                Password = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.Is<HashedPassword>(y => y.Password == command.Password)))
                .Returns(Task.FromResult(FakeData.String()));
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.GetAsync<User>(command.Principal.Identity.GetUserId<int>()))
                .Returns(Task.FromResult(new User()));
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleCreateLocalMembershipCommand(
                queries.Object, commands.Object, entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.GetAsync<User>(command.Principal.Identity.GetUserId<int>()), Times.Once);
            commands.Verify(x => x.Execute(It.IsAny<CreateUser>()), Times.Never);
            commands.Verify(x => x.Execute(It.IsAny<RedeemEmailVerification>()), Times.Never);
        }
    }
}
