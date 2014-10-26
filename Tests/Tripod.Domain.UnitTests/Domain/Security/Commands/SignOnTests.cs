using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class SignOnTests
    {
        [Fact]
        public void Handler_SetsSignedOn_WhenPrincipal_IsAlreadyAuthenticated()
        {
            var userId = FakeData.Id();
            var userName = FakeData.String();
            var command = new SignOn
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)), 
                }, "authenticationType"), null),
            };
            User user = new ProxiedUser(userId) { Name = userName };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.Is<UserBy>(y => y.Principal == command.Principal)))
                .Returns(Task.FromResult(user));
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            var handler = new HandleSignOnCommand(queries.Object, commands.Object,
                entities.Object, authenticator.Object);

            command.SignedOn.ShouldBeNull();
            handler.Handle(command).Wait();

            command.SignedOn.ShouldNotBeNull();
            command.SignedOn.ShouldEqual(user);
        }

        [Fact]
        public void Handler_DoesNotAuthenticate_WhenNoRemoteMemberhipTicketExists()
        {
            var userId = FakeData.Id();
            var userName = FakeData.String();
            var command = new SignOn
            {
                Principal = new GenericPrincipal(new GenericIdentity(""), null),
            };
            User user = new ProxiedUser(userId) { Name = userName };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserBy, bool>> expectedUserByQuery = x => x.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedUserByQuery)))
                .Returns(Task.FromResult(user));
            Expression<Func<PrincipalRemoteMembershipTicket, bool>> expectedRemoteMembershipTicketQuery =
                x => x.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedRemoteMembershipTicketQuery)))
                .Returns(Task.FromResult(null as RemoteMembershipTicket));
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            var handler = new HandleSignOnCommand(queries.Object, commands.Object,
                entities.Object, authenticator.Object);

            handler.Handle(command).Wait();

            authenticator.Verify(x => x.SignOn(It.IsAny<User>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void Handler_Authenticates_WhenUserByExternalLogin_IsFound()
        {
            var userId = FakeData.Id();
            var userName = FakeData.String();
            var loginProvider = FakeData.String();
            var providerKey = FakeData.String();
            var command = new SignOn
            {
                Principal = new GenericPrincipal(new GenericIdentity(""), null),
            };
            User user = new ProxiedUser(userId) { Name = userName };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserBy, bool>> expectedUserByPrincipalQuery =
                x => x.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedUserByPrincipalQuery)))
                .Returns(Task.FromResult(user));
            Expression<Func<PrincipalRemoteMembershipTicket, bool>> expectedRemoteMembershipTicketQuery =
                x => x.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedRemoteMembershipTicketQuery)))
                .Returns(Task.FromResult(new RemoteMembershipTicket
                {
                    Login = new UserLoginInfo(loginProvider, providerKey),
                    UserName = userName,
                }));
            Expression<Func<UserBy, bool>> expectedUserByLoginInfoQuery =
                x => x.UserLoginInfo.LoginProvider == loginProvider
                    && x.UserLoginInfo.ProviderKey == providerKey;
            queries.Setup(x => x.Execute(It.Is(expectedUserByLoginInfoQuery)))
                .Returns(Task.FromResult(user));
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            authenticator.Setup(x => x.SignOn(user, command.IsPersistent))
                .Returns(Task.FromResult(0));
            var handler = new HandleSignOnCommand(queries.Object, commands.Object,
                entities.Object, authenticator.Object);

            command.SignedOn.ShouldBeNull();
            handler.Handle(command).Wait();

            authenticator.Verify(x => x.SignOn(user, command.IsPersistent), Times.Once);
            command.SignedOn.ShouldNotBeNull();
            command.SignedOn.ShouldEqual(user);
        }

        [Fact]
        public void Handler_Authenticates_WhenEmailAddress_IsFoundByClaim()
        {
            var userId = FakeData.Id();
            var userName = FakeData.String();
            var loginProvider = FakeData.String();
            var providerKey = FakeData.String();
            var command = new SignOn
            {
                Principal = new GenericPrincipal(new GenericIdentity(""), null),
            };
            User user = new ProxiedUser(userId) { Name = userName };
            var emailAddress = new EmailAddress
            {
                IsVerified = true,
                Value = FakeData.Email(),
                User = user,
                UserId = userId,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<UserBy, bool>> expectedUserByPrincipalQuery =
                x => x.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedUserByPrincipalQuery)))
                .Returns(Task.FromResult(user));
            Expression<Func<PrincipalRemoteMembershipTicket, bool>> expectedRemoteMembershipTicketQuery =
                x => x.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedRemoteMembershipTicketQuery)))
                .Returns(Task.FromResult(new RemoteMembershipTicket
                {
                    Login = new UserLoginInfo(loginProvider, providerKey),
                    UserName = userName,
                }));
            Expression<Func<UserBy, bool>> expectedUserByLoginInfoQuery =
                x => x.UserLoginInfo.LoginProvider == loginProvider
                    && x.UserLoginInfo.ProviderKey == providerKey;
            queries.Setup(x => x.Execute(It.Is(expectedUserByLoginInfoQuery)))
                .Returns(Task.FromResult(null as User));
            var emailClaim = new Claim(ClaimTypes.Email, emailAddress.Value);
            Expression<Func<ExternalCookieClaim, bool>> expectedExternalCookieQuery =
                x => x.ClaimType == ClaimTypes.Email &&
                    x.AuthenticationType == DefaultAuthenticationTypes.ExternalCookie;
            queries.Setup(x => x.Execute(It.Is(expectedExternalCookieQuery)))
                .Returns(Task.FromResult(emailClaim));
            queries.Setup(x => x.Execute(It.Is<EmailAddressBy>(y => y.Claim == emailClaim)))
                .Returns(Task.FromResult(emailAddress));
            var commands = new Mock<IProcessCommands>(MockBehavior.Strict);
            Expression<Func<CreateRemoteMembership, bool>> expectedCreateRemoteMembershipCommand =
                x => x.Principal == command.Principal && x.User.Equals(user);
            commands.Setup(x => x.Execute(It.Is(expectedCreateRemoteMembershipCommand)))
                .Returns(Task.FromResult(0));
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Update(user));
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            authenticator.Setup(x => x.SignOn(user, command.IsPersistent))
                .Returns(Task.FromResult(0));
            var handler = new HandleSignOnCommand(queries.Object, commands.Object,
                entities.Object, authenticator.Object);

            command.SignedOn.ShouldBeNull();
            handler.Handle(command).Wait();

            entities.Verify(x => x.Update(user), Times.Once);
            commands.Verify(x => x.Execute(It.Is(expectedCreateRemoteMembershipCommand)), Times.Once);
            authenticator.Verify(x => x.SignOn(user, command.IsPersistent), Times.Once);
            command.SignedOn.ShouldNotBeNull();
            command.SignedOn.ShouldEqual(user);
        }
    }
}
