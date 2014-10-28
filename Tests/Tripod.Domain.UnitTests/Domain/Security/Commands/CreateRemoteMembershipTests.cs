using System;
using System.Data.Entity;
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
    public class CreateRemoteMembershipTests : FluentValidationTests
    {
        [Fact]
        public void Validator_Principal_IsInvalid_WhenNoRemoteMembershipTicketExists()
        {
            var command = new CreateRemoteMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity(""), null),
                UserName = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<PrincipalRemoteMembershipTicket>()))
                .Returns(Task.FromResult(null as RemoteMembershipTicket));
            Expression<Func<UserBy, bool>> expectedUserByQuery = y => y.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(expectedUserByQuery)))
                .Returns(Task.FromResult(null as User));
            var validator = new ValidateCreateRemoteMembershipCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_RemoteMembership_NoTicket
            );
            queries.Verify(x => x.Execute(It.IsAny<PrincipalRemoteMembershipTicket>()), Times.Once);
        }

        [Fact]
        public void Validator_UserName_IsInvalid_WhenUserName_IsUnverifiedEmail()
        {
            var command = new CreateRemoteMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity(""), null),
                UserName = FakeData.Email(),
                Ticket = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<PrincipalRemoteMembershipTicket>()))
                .Returns(Task.FromResult(null as RemoteMembershipTicket));
            Expression<Func<UserBy, bool>> expectedUserByQuery = y => y.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(expectedUserByQuery)))
                .Returns(Task.FromResult(null as User));
            Expression<Func<EmailVerificationBy, bool>> expectedEmailVerificationByTicketQuery =
                x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedEmailVerificationByTicketQuery)))
                .Returns(Task.FromResult(new EmailVerification
                {
                    EmailAddress = new EmailAddress
                    {
                        Value = FakeData.Email(),
                    },
                }));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationTokenIsValid>()))
                .Returns(Task.FromResult(false));
            var validator = new ValidateCreateRemoteMembershipCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_UserName_AllowedEmailAddress
                .Replace("{PropertyName}", User.Constraints.NameLabel.ToLower())
                .Replace("{PropertyValue}", command.UserName)
            );
            queries.Verify(x => x.Execute(It.Is(expectedEmailVerificationByTicketQuery)), Times.AtLeast(1));
        }

        [Fact]
        public void Validator_UserName_IsValid_WhenEmpty_AndPrincipalIsAuthenticated_AndUserIsNull()
        {
            var command = new CreateRemoteMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity(FakeData.String()), null),
                UserName = null,
                Ticket = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<PrincipalRemoteMembershipTicket>()))
                .Returns(Task.FromResult(null as RemoteMembershipTicket));
            Expression<Func<UserBy, bool>> expectedUserByQuery = y => y.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(expectedUserByQuery)))
                .Returns(Task.FromResult(null as User));
            Expression<Func<EmailVerificationBy, bool>> expectedEmailVerificationByTicketQuery =
                x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedEmailVerificationByTicketQuery)))
                .Returns(Task.FromResult(new EmailVerification
                {
                    EmailAddress = new EmailAddress
                    {
                        Value = FakeData.Email(),
                    },
                }));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationTokenIsValid>()))
                .Returns(Task.FromResult(false));
            var validator = new ValidateCreateRemoteMembershipCommand(queries.Object);

            var result = validator.Validate(command);

            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(targetError).ShouldEqual(0);
        }

        [Fact]
        public void Validator_Ticket_IsInvalid_WhenPurpose_IsNotCreateRemoteUser()
        {
            var command = new CreateRemoteMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity("", ""), null),
                Ticket = FakeData.String(),
                UserName = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<PrincipalRemoteMembershipTicket>()))
                .Returns(Task.FromResult(null as RemoteMembershipTicket));
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(null as User));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationBy>()))
                .Returns(Task.FromResult(new EmailVerification
                {
                    Ticket = command.Ticket,
                    Purpose = FakeData.OneOf(EmailVerificationPurpose.AddEmail, EmailVerificationPurpose.CreateLocalUser,
                        EmailVerificationPurpose.ForgotPassword, EmailVerificationPurpose.Invalid),
                    ExpiresOnUtc = DateTime.UtcNow.AddMinutes(5),
                }));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationTokenIsValid>()))
                .Returns(Task.FromResult(false));
            var validator = new ValidateCreateRemoteMembershipCommand(queries.Object);

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
        public void Validator_Ticket_IsInvalid_WhenTokenIsNotValid()
        {
            var command = new CreateRemoteMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity("", ""), null),
                UserName = FakeData.String(),
                Ticket = FakeData.String(),
                Token = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<PrincipalRemoteMembershipTicket>()))
                .Returns(Task.FromResult(null as RemoteMembershipTicket));
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(null as User));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationBy>()))
                .Returns(Task.FromResult(new EmailVerification
                {
                    Ticket = command.Ticket,
                    Purpose = EmailVerificationPurpose.CreateRemoteUser,
                    ExpiresOnUtc = DateTime.UtcNow.AddMinutes(5),
                }));
            queries.Setup(x => x.Execute(It.IsAny<EmailVerificationTokenIsValid>()))
                .Returns(Task.FromResult(false));
            var validator = new ValidateCreateRemoteMembershipCommand(queries.Object);

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
        public void Handler_DoesNotLoadUser_WhenPrincipalHasNoAppUserId()
        {
            var command = new CreateRemoteMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity(""), null),
                UserName = FakeData.String(),
                Ticket = FakeData.String(),
                Token = FakeData.String(),
                User = new User(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<PrincipalRemoteMembershipTicket>()))
                .Returns(Task.FromResult(new RemoteMembershipTicket
                {
                    Login = new UserLoginInfo(FakeData.String(), FakeData.String()),
                    UserName = FakeData.String(),
                }));
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleCreateRemoteMembershipCommand(
                queries.Object, commands.Object, entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<User>(), Times.Never);
        }

        [Fact]
        public void Handler_LoadsUser_WhenPrincipalHasAppUserId()
        {
            var command = new CreateRemoteMembership
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.Id().ToString(CultureInfo.InvariantCulture)),
                }, "authenticationType"), null),
                UserName = FakeData.String(),
                Ticket = FakeData.String(),
                Token = FakeData.String(),
                User = new User(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<PrincipalRemoteMembershipTicket>()))
                .Returns(Task.FromResult(new RemoteMembershipTicket
                {
                    Login = new UserLoginInfo(FakeData.String(), FakeData.String()),
                    UserName = FakeData.String(),
                }));
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            commands.Setup(x => x.Execute(It.Is<CreateUser>(y => y.Name == command.UserName)))
                .Callback<IDefineCommand>(x => ((CreateUser)x).CreatedEntity = new User())
                .Returns(Task.FromResult(0));
            Expression<Func<RedeemEmailVerification, bool>> expectedRedeemEmailVerificationCommand =
                x => x.Commit == false && x.Ticket == command.Ticket && x.Token == command.Token;
            commands.Setup(x => x.Execute(It.Is(expectedRedeemEmailVerificationCommand)))
                .Returns(Task.FromResult(0));
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(new User[0].AsQueryable());
            entities.Setup(x => x.Get<User>()).Returns(dbSet.Object);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleCreateRemoteMembershipCommand(
                queries.Object, commands.Object, entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<User>(), Times.Once);
        }

        [Fact]
        public void Handler_DoesNotCreateRemoteMembership_WhenRemoteLoginAlreadyExists()
        {
            var command = new CreateRemoteMembership
            {
                Principal = new GenericPrincipal(new GenericIdentity(""), null),
                UserName = FakeData.String(),
                Ticket = FakeData.String(),
                Token = FakeData.String(),
                User = new User(),
            };
            var remoteMembershipTicket = new RemoteMembershipTicket
            {
                Login = new UserLoginInfo(FakeData.String(), FakeData.String()),
                UserName = FakeData.String(),
            };
            command.User.RemoteMemberships.Add(new ProxiedRemoteMembership(
                remoteMembershipTicket.Login.LoginProvider,
                remoteMembershipTicket.Login.ProviderKey));
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.IsAny<PrincipalRemoteMembershipTicket>()))
                .Returns(Task.FromResult(remoteMembershipTicket));
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleCreateRemoteMembershipCommand(
                queries.Object, commands.Object, entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}
