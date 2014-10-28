using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class RedeemEmailVerificationTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenUserIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new RedeemEmailVerification(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("user");
        }

        [Theory]
        [InlineData(null), InlineData(""), InlineData(" ")]
        public void Validate_TicketIsInvalid_WhenEmpty(string ticket)
        {
            var command = new RedeemEmailVerification();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateRedeemEmailVerificationCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Ticket);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .notempty_error
                .Replace("{PropertyName}", EmailVerification.Constraints.Label)
            );
        }

        [Fact]
        public void Validate_TicketIsInvalid_WhenTokenIsNotValid()
        {
            var command = new RedeemEmailVerification
            {
                Ticket = FakeData.String(),
                Token = FakeData.String(),
            };
            var emailVerification = new EmailVerification
            {
                Ticket = command.Ticket,
                Token = FakeData.String(),
                ExpiresOnUtc = DateTime.UtcNow.AddMinutes(5),
                Purpose = FakeData.OneOf(EmailVerificationPurpose.CreateRemoteUser,
                    EmailVerificationPurpose.CreateLocalUser, EmailVerificationPurpose.AddEmail)
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<EmailVerificationBy, bool>> expectedEmailVerificationQuery =
                x => x.Ticket == command.Ticket;
            queries.Setup(x => x.Execute(It.Is(expectedEmailVerificationQuery)))
                .Returns(Task.FromResult(emailVerification));
            Expression<Func<EmailVerificationTokenIsValid, bool>> expectedTokenValidationQuery =
                x => x.Token == command.Token && x.Ticket == command.Ticket
                    && x.Purpose == emailVerification.Purpose;
            queries.Setup(x => x.Execute(It.Is(expectedTokenValidationQuery)))
                .Returns(Task.FromResult(false));
            var validator = new ValidateRedeemEmailVerificationCommand(queries.Object);

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
        public void Handler_LoadsVerfication_ByTicket()
        {
            var command = new RedeemEmailVerification
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.IdString()),
                }, "authenticationType"), null),
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
            var user = new ProxiedUser(command.Principal.Identity.GetUserId<int>());
            user.EmailAddresses.Add(new EmailAddress { IsPrimary = true });
            user.EmailAddresses.Add(new EmailAddress { IsPrimary = false });
            user.EmailAddresses.Add(new EmailAddress { IsPrimary = false });
            User[] userData =
            {
                new ProxiedUser(FakeData.Id()),
                user,
                new ProxiedUser(FakeData.Id()),
            };
            var userSet = new Mock<DbSet<User>>(MockBehavior.Strict)
                .SetupDataAsync(userData.AsQueryable());
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Get<EmailVerification>()).Returns(emailVerificationSet.Object);
            entities.Setup(x => x.Get<User>()).Returns(userSet.Object);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleRedeemEmailVerificationCommand(entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<EmailVerification>(), Times.Exactly(2));
        }
    }
}
