using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class EmailVerificationTokenIsValidTests
    {
        [Theory]
        [InlineData(EmailVerificationPurpose.AddEmail)]
        [InlineData(EmailVerificationPurpose.CreateLocalUser)]
        [InlineData(EmailVerificationPurpose.CreateRemoteUser)]
        [InlineData(EmailVerificationPurpose.ForgotPassword)]
        public void Ctor_SetsProperties(EmailVerificationPurpose purpose)
        {
            var token = Guid.NewGuid().ToString();
            var ticket = Guid.NewGuid().ToString();
            var query = new EmailVerificationTokenIsValid(token, ticket, purpose);
            query.Token.ShouldEqual(token);
            query.Ticket.ShouldEqual(ticket);
            query.Purpose.ShouldEqual(purpose);
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.AddEmail)]
        [InlineData(EmailVerificationPurpose.CreateLocalUser)]
        [InlineData(EmailVerificationPurpose.CreateRemoteUser)]
        [InlineData(EmailVerificationPurpose.ForgotPassword)]
        public void Handler_ReturnsFalse_WhenTokenIsInvalid(EmailVerificationPurpose purpose)
        {
            string token = Guid.NewGuid().ToString();
            string ticket = Guid.NewGuid().ToString();
            var command = new EmailVerificationTokenIsValid(token, ticket, purpose);
            var userStore = new Mock<IUserStore<UserTicket, string>>(MockBehavior.Strict);
            var userManager = new UserManager<UserTicket, string>(userStore.Object);
            var tokenProvider = new Mock<IUserTokenProvider<UserTicket, string>>(MockBehavior.Strict);
            tokenProvider.Setup(x => x.ValidateAsync(purpose.ToString(), token, userManager, It.Is<UserTicket>(y => y.UserName == ticket)))
                .Returns(Task.FromResult(false));
            userManager.UserTokenProvider = tokenProvider.Object;
            var handler = new HandleEmailVerificationTokenIsValidQuery(userManager);

            bool result = handler.Handle(command).Result;

            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.AddEmail, "")]
        [InlineData(EmailVerificationPurpose.CreateLocalUser, null)]
        [InlineData(EmailVerificationPurpose.CreateRemoteUser, " \r\n")]
        [InlineData(EmailVerificationPurpose.ForgotPassword, " \t\t\t  ")]
        public void Handler_ReturnsFalse_WhenTokenIsEmpty(EmailVerificationPurpose purpose, string token)
        {
            string ticket = Guid.NewGuid().ToString();
            var command = new EmailVerificationTokenIsValid(token, ticket, purpose);
            var userStore = new Mock<IUserStore<UserTicket, string>>(MockBehavior.Strict);
            var userManager = new UserManager<UserTicket, string>(userStore.Object);
            var tokenProvider = new Mock<IUserTokenProvider<UserTicket, string>>(MockBehavior.Strict);
            tokenProvider.Setup(x => x.ValidateAsync(purpose.ToString(), token, userManager, It.Is<UserTicket>(y => y.UserName == ticket)))
                .Returns(Task.FromResult(true));
            userManager.UserTokenProvider = tokenProvider.Object;
            var handler = new HandleEmailVerificationTokenIsValidQuery(userManager);

            bool result = handler.Handle(command).Result;

            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.AddEmail, "")]
        [InlineData(EmailVerificationPurpose.CreateLocalUser, null)]
        [InlineData(EmailVerificationPurpose.CreateRemoteUser, " \r\n")]
        [InlineData(EmailVerificationPurpose.ForgotPassword, " \t\t\t  ")]
        public void Handler_ReturnsFalse_WhenTicketIsEmpty(EmailVerificationPurpose purpose, string ticket)
        {
            string token = Guid.NewGuid().ToString();
            var command = new EmailVerificationTokenIsValid(token, ticket, purpose);
            var userStore = new Mock<IUserStore<UserTicket, string>>(MockBehavior.Strict);
            var userManager = new UserManager<UserTicket, string>(userStore.Object);
            var tokenProvider = new Mock<IUserTokenProvider<UserTicket, string>>(MockBehavior.Strict);
            tokenProvider.Setup(x => x.ValidateAsync(purpose.ToString(), token, userManager, It.Is<UserTicket>(y => y.UserName == ticket)))
                .Returns(Task.FromResult(true));
            userManager.UserTokenProvider = tokenProvider.Object;
            var handler = new HandleEmailVerificationTokenIsValidQuery(userManager);

            bool result = handler.Handle(command).Result;

            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.AddEmail)]
        [InlineData(EmailVerificationPurpose.CreateLocalUser)]
        [InlineData(EmailVerificationPurpose.CreateRemoteUser)]
        [InlineData(EmailVerificationPurpose.ForgotPassword)]
        public void Handler_ReturnsTrue_WhenTokenIsValid(EmailVerificationPurpose purpose)
        {
            string token = Guid.NewGuid().ToString();
            string ticket = Guid.NewGuid().ToString();
            var command = new EmailVerificationTokenIsValid(token, ticket, purpose);
            var userStore = new Mock<IUserStore<UserTicket, string>>(MockBehavior.Strict);
            var userManager = new UserManager<UserTicket, string>(userStore.Object);
            var tokenProvider = new Mock<IUserTokenProvider<UserTicket, string>>(MockBehavior.Strict);
            tokenProvider.Setup(x => x.ValidateAsync(purpose.ToString(), token, userManager, It.Is<UserTicket>(y => y.UserName == ticket)))
                .Returns(Task.FromResult(true));
            userManager.UserTokenProvider = tokenProvider.Object;
            var handler = new HandleEmailVerificationTokenIsValidQuery(userManager);

            bool result = handler.Handle(command).Result;

            result.ShouldBeTrue();
        }
    }
}
