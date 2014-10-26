using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class CreateEmailVerificationTests : FluentValidationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Validator_IsInvalid_WhenEmailAddress_IsEmpty(string emailAddress)
        {
            var command = new CreateEmailVerification
            {
                Purpose = EmailVerificationPurpose.ForgotPassword,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateCreateEmailVerificationCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.EmailAddress);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .notempty_error
                .Replace("{PropertyName}", EmailAddress.Constraints.Label));
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddress, command);
        }

        [Fact]
        public void Validator_IsValid_WhenPurposeIsForgotPassword_AndUserByVerifiedEmail_IsFound()
        {
            var emailAddress = FakeData.Email();
            var command = new CreateEmailVerification
            {
                EmailAddress = emailAddress,
                Purpose = EmailVerificationPurpose.ForgotPassword,
            };
            var user = new User();
            var entity = new ProxiedEmailAddress(FakeData.Id())
            {
                IsVerified = true,
                Value = emailAddress,
                User = user,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Value == emailAddress;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity as EmailAddress));

            var validator = new ValidateCreateEmailVerificationCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            validator.ShouldNotHaveValidationErrorFor(x => x.EmailAddress, command);
        }

        [Fact]
        public void Handler_LoadsEmailAddress_ByCommandValue()
        {
            var emailAddress = FakeData.Email();
            var command = new CreateEmailVerification
            {
                EmailAddress = emailAddress,
                Purpose = FakeData.OneOf(EmailVerificationPurpose.AddEmail, EmailVerificationPurpose.CreateLocalUser,
                    EmailVerificationPurpose.CreateRemoteUser, EmailVerificationPurpose.ForgotPassword)
            };
            var emailAddressSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict)
                .SetupDataAsync(new EmailAddress[0].AsQueryable());
            var emailVerificationSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict)
                .SetupDataAsync(new EmailVerification[0].AsQueryable());
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.Is<HashedEmailValueBy>(y => y.EmailAddress == emailAddress)))
                .Returns(Task.FromResult(FakeData.String()));
            queries.Setup(x => x.Execute(It.IsAny<RandomSecret>())).Returns(FakeData.String());
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Get<EmailAddress>()).Returns(emailAddressSet.Object);
            entities.Setup(x => x.Query<EmailVerification>()).Returns(emailVerificationSet.Object);
            entities.Setup(x => x.Create(It.IsAny<EmailVerification>()));
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var userStore = new Mock<IUserStore<UserTicket, string>>(MockBehavior.Strict);
            var tokenProvider = new Mock<IUserTokenProvider<UserTicket, string>>(MockBehavior.Strict);
            var userManager = new UserManager<UserTicket, string>(userStore.Object);
            tokenProvider.Setup(x => x.GenerateAsync(command.Purpose.ToString(), userManager, It.IsAny<UserTicket>()))
                .Returns(Task.FromResult(FakeData.String()));
            userManager.UserTokenProvider = tokenProvider.Object;
            var handler = new HandleCreateEmailVerificationCommand(userManager, queries.Object, entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<EmailAddress>(), Times.Once);
            queries.Verify(x => x.Execute(It.Is<HashedEmailValueBy>(y => y.EmailAddress == emailAddress)), Times.Once);
        }

        [Fact]
        public void Handler_EnsuresVerificationTicket_IsUnique()
        {
            var secretGenerationIndex = 0;
            var firstTicket = FakeData.String();
            var secondTicket = FakeData.String();
            var emailAddress = FakeData.Email();
            var command = new CreateEmailVerification
            {
                EmailAddress = emailAddress,
                Purpose = FakeData.OneOf(EmailVerificationPurpose.AddEmail, EmailVerificationPurpose.CreateLocalUser,
                    EmailVerificationPurpose.CreateRemoteUser, EmailVerificationPurpose.ForgotPassword)
            };
            var emailAddressSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict)
                .SetupDataAsync(new EmailAddress[0].AsQueryable());
            var emailVerificationData = new[]
            {
                new EmailVerification { Ticket = firstTicket, },
            };
            var emailVerificationSet = new Mock<DbSet<EmailVerification>>(MockBehavior.Strict)
                .SetupDataAsync(emailVerificationData.AsQueryable());
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.Is<HashedEmailValueBy>(y => y.EmailAddress == emailAddress)))
                .Returns(Task.FromResult(FakeData.String()));
            queries.Setup(x => x.Execute(It.IsAny<RandomSecret>()))
                .Returns(() => secretGenerationIndex++ < 5 ? firstTicket : secondTicket);
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Get<EmailAddress>()).Returns(emailAddressSet.Object);
            entities.Setup(x => x.Query<EmailVerification>()).Returns(emailVerificationSet.Object);
            entities.Setup(x => x.Create(It.IsAny<EmailVerification>()));
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var userStore = new Mock<IUserStore<UserTicket, string>>(MockBehavior.Strict);
            var tokenProvider = new Mock<IUserTokenProvider<UserTicket, string>>(MockBehavior.Strict);
            var userManager = new UserManager<UserTicket, string>(userStore.Object);
            tokenProvider.Setup(x => x.GenerateAsync(command.Purpose.ToString(), userManager, It.IsAny<UserTicket>()))
                .Returns(Task.FromResult(FakeData.String()));
            userManager.UserTokenProvider = tokenProvider.Object;
            var handler = new HandleCreateEmailVerificationCommand(userManager, queries.Object, entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<EmailAddress>(), Times.Once);
            queries.Verify(x => x.Execute(It.Is<HashedEmailValueBy>(y => y.EmailAddress == emailAddress)), Times.Once);
        }
    }
}
