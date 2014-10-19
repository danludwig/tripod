using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;
using Assert = Should.Core.Assertions.Assert;

namespace Tripod.Domain.Security
{
    public class PrincipalRemoteMembershipTicketTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenPrincipalIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new PrincipalRemoteMembershipTicket(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("principal");
        }

        [Fact]
        public void Ctor_SetsPrincipalsProperty_WhenPrincipalIsNotNull()
        {
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var query = new PrincipalRemoteMembershipTicket(principal.Object);
            query.Principal.ShouldEqual(principal.Object);
        }

        [Fact]
        public void Handle_ReturnsRemoteMembershipTicket_UsingAuthenticatorMethod()
        {
            var userName = Guid.NewGuid().ToString();
            var loginProvider = Guid.NewGuid().ToString();
            var providerKey = Guid.NewGuid().ToString();
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var remoteMembershipTicket = new RemoteMembershipTicket
            {
                UserName = userName,
                Login = new UserLoginInfo(loginProvider, providerKey),
            };
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            authenticator.Setup(x => x.GetRemoteMembershipTicket(principal.Object))
                .Returns(Task.FromResult(remoteMembershipTicket));
            var handler = new HandlePrincipalRemoteMembershipTicketQuery(authenticator.Object);
            var query = new PrincipalRemoteMembershipTicket(principal.Object);

            RemoteMembershipTicket result = handler.Handle(query).Result;

            result.ShouldEqual(remoteMembershipTicket);
        }
    }
}
