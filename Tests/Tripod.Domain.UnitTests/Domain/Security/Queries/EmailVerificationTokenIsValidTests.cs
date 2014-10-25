using System;
using System.Linq.Expressions;
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
        public void Query_Ctor_SetsProperties(EmailVerificationPurpose purpose)
        {
            var token = FakeData.String();
            var ticket = FakeData.String();
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
            string token = FakeData.String();
            string ticket = FakeData.String();
            var command = new EmailVerificationTokenIsValid(token, ticket, purpose);
            var userStore = new Mock<IUserStore<UserTicket, string>>(MockBehavior.Strict);
            var userManager = new UserManager<UserTicket, string>(userStore.Object);
            var tokenProvider = new Mock<IUserTokenProvider<UserTicket, string>>(MockBehavior.Strict);
            Expression<Func<IUserTokenProvider<UserTicket, string>, Task<bool>>> expectedMethod =
                x => x.ValidateAsync(purpose.ToString(), token, userManager,
                    It.Is<UserTicket>(y => y.UserName == ticket));
            tokenProvider.Setup(expectedMethod)
                .Returns(Task.FromResult(false));
            userManager.UserTokenProvider = tokenProvider.Object;
            var handler = new HandleEmailVerificationTokenIsValidQuery(userManager);

            bool result = handler.Handle(command).Result;

            result.ShouldBeFalse();
            tokenProvider.Verify(expectedMethod, Times.Once);
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.AddEmail, "")]
        [InlineData(EmailVerificationPurpose.CreateLocalUser, null)]
        [InlineData(EmailVerificationPurpose.CreateRemoteUser, " \r\n")]
        [InlineData(EmailVerificationPurpose.ForgotPassword, " \t\t\t  ")]
        public void Handler_ReturnsFalse_WhenTokenIsEmpty(EmailVerificationPurpose purpose, string token)
        {
            string ticket = FakeData.String();
            var command = new EmailVerificationTokenIsValid(token, ticket, purpose);
            var userStore = new Mock<IUserStore<UserTicket, string>>(MockBehavior.Strict);
            var userManager = new UserManager<UserTicket, string>(userStore.Object);
            var tokenProvider = new Mock<IUserTokenProvider<UserTicket, string>>(MockBehavior.Strict);
            Expression<Func<IUserTokenProvider<UserTicket, string>, Task<bool>>> expectedMethod =
                x => x.ValidateAsync(purpose.ToString(), token, userManager,
                    It.Is<UserTicket>(y => y.UserName == ticket));
            tokenProvider.Setup(expectedMethod)
                .Returns(Task.FromResult(true));
            userManager.UserTokenProvider = tokenProvider.Object;
            var handler = new HandleEmailVerificationTokenIsValidQuery(userManager);

            bool result = handler.Handle(command).Result;

            result.ShouldBeFalse();
            tokenProvider.Verify(expectedMethod, Times.Never);
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.AddEmail, "")]
        [InlineData(EmailVerificationPurpose.CreateLocalUser, null)]
        [InlineData(EmailVerificationPurpose.CreateRemoteUser, " \r\n")]
        [InlineData(EmailVerificationPurpose.ForgotPassword, " \t\t\t  ")]
        public void Handler_ReturnsFalse_WhenTicketIsEmpty(EmailVerificationPurpose purpose, string ticket)
        {
            string token = FakeData.String();
            var command = new EmailVerificationTokenIsValid(token, ticket, purpose);
            var userStore = new Mock<IUserStore<UserTicket, string>>(MockBehavior.Strict);
            var userManager = new UserManager<UserTicket, string>(userStore.Object);
            var tokenProvider = new Mock<IUserTokenProvider<UserTicket, string>>(MockBehavior.Strict);
            Expression<Func<IUserTokenProvider<UserTicket, string>, Task<bool>>> expectedMethod =
                x => x.ValidateAsync(purpose.ToString(), token, userManager,
                    It.Is<UserTicket>(y => y.UserName == ticket));
            tokenProvider.Setup(expectedMethod)
                .Returns(Task.FromResult(true));
            userManager.UserTokenProvider = tokenProvider.Object;
            var handler = new HandleEmailVerificationTokenIsValidQuery(userManager);

            bool result = handler.Handle(command).Result;

            result.ShouldBeFalse();
            tokenProvider.Verify(expectedMethod, Times.Never);
        }

        [Theory]
        [InlineData(EmailVerificationPurpose.AddEmail)]
        [InlineData(EmailVerificationPurpose.CreateLocalUser)]
        [InlineData(EmailVerificationPurpose.CreateRemoteUser)]
        [InlineData(EmailVerificationPurpose.ForgotPassword)]
        public void Handler_ReturnsTrue_WhenTokenIsValid(EmailVerificationPurpose purpose)
        {
            string token = FakeData.String();
            string ticket = FakeData.String();
            var command = new EmailVerificationTokenIsValid(token, ticket, purpose);
            var userStore = new Mock<IUserStore<UserTicket, string>>(MockBehavior.Strict);
            var userManager = new UserManager<UserTicket, string>(userStore.Object);
            var tokenProvider = new Mock<IUserTokenProvider<UserTicket, string>>(MockBehavior.Strict);
            Expression<Func<IUserTokenProvider<UserTicket, string>, Task<bool>>> expectedMethod =
                x => x.ValidateAsync(purpose.ToString(), token, userManager,
                    It.Is<UserTicket>(y => y.UserName == ticket));
            tokenProvider.Setup(expectedMethod)
                .Returns(Task.FromResult(true));
            userManager.UserTokenProvider = tokenProvider.Object;
            var handler = new HandleEmailVerificationTokenIsValidQuery(userManager);

            bool result = handler.Handle(command).Result;

            result.ShouldBeTrue();
            tokenProvider.Verify(expectedMethod, Times.Once);
        }
    }
}
