using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class MustNotBeUnverifiedEmailUserNameTests : FluentValidationTests
    {
        #region With Ticket

        [Fact]
        public void TicketCtor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustNotBeUnverifiedEmailUserName<object>(null, null as Func<object, string>));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void TicketCtor_ThrowsArgumentNullException_WhenTicket_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustNotBeUnverifiedEmailUserName<object>(queries.Object, null as Func<object, string>));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("ticket");
        }

        [Theory]
        [InlineData("email@domain1.tld", "email@domain2.tld")]
        [InlineData("email1@domain.tld", "email2@domain.tld")]
        [InlineData("email@domain.com", "email@domain.net")]
        public void IsInvalid_WhenEmailVerificationByTicket_HasEmailNotMatchingUserName(
            string userName, string ticketEmail)
        {
            string ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            EmailAddress emailAddress = new ProxiedEmailAddress(FakeData.Id())
            {
                Value = ticketEmail,
            };
            EmailVerification verification = new ProxiedEmailVerification(FakeData.Id())
            {
                Ticket = ticket,
                EmailAddress = emailAddress,
            };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(verification));
            var command = new FakeMustNotBeUnverifiedEmailUserNameCommand
            {
                UserName = userName,
                Ticket = ticket,
            };
            var validator = new FakeMustNotBeUnverifiedEmailUserNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .Validation_UserName_AllowedEmailAddress
                .Replace("{PropertyName}", User.Constraints.NameLabel.ToLower())
                .Replace("{PropertyValue}", userName)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public void IsValid_WhenUserName_IsEmpty_WithTicket(string userName)
        {
            string ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBeUnverifiedEmailUserNameCommand
            {
                UserName = userName,
                Ticket = ticket,
            };
            var validator = new FakeMustNotBeUnverifiedEmailUserNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
        }

        [Theory]
        [InlineData("username@@domain.tld")]
        [InlineData("username@domain..tld")]
        [InlineData("username@domaintld")]
        public void IsValid_WhenUserName_DoesNotMatchEmailRegEx_WithTicket(string userName)
        {
            string ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBeUnverifiedEmailUserNameCommand
            {
                UserName = userName,
                Ticket = ticket,
            };
            var validator = new FakeMustNotBeUnverifiedEmailUserNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
        }

        [Fact]
        public void IsValid_WhenEmailVerificationByTicket_IsNotFound()
        {
            string userName = FakeData.Email();
            string ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(null as EmailVerification));
            var command = new FakeMustNotBeUnverifiedEmailUserNameCommand
            {
                UserName = userName,
                Ticket = ticket,
            };
            var validator = new FakeMustNotBeUnverifiedEmailUserNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
        }

        [Theory]
        [InlineData("email1@domain.tld", "Email1@Domain.Tld")]
        [InlineData("email2@domain.tld", "email2@domain.tld")]
        [InlineData("Email3@Domain.Tld", "email3@domain.tld")]
        public void IsValid_WhenEmailVerificationByTicket_HasEmailMatchingUserName_CaseInsensitively(
            string userName, string ticketEmail)
        {
            string ticket = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            EmailAddress emailAddress = new ProxiedEmailAddress(FakeData.Id())
            {
                Value = ticketEmail,
            };
            EmailVerification verification = new ProxiedEmailVerification(FakeData.Id())
            {
                Ticket = ticket,
                EmailAddress = emailAddress,
            };
            Expression<Func<EmailVerificationBy, bool>> expectedQuery = x => x.Ticket == ticket;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(verification));
            var command = new FakeMustNotBeUnverifiedEmailUserNameCommand
            {
                UserName = userName,
                Ticket = ticket,
            };
            var validator = new FakeMustNotBeUnverifiedEmailUserNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
        }

        #endregion
        #region With UserId

        [Fact]
        public void UserIdCtor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustNotBeUnverifiedEmailUserName<object>(null, null as Func<object, int>));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void UserIdCtor_ThrowsArgumentNullException_WhenTicket_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var exception = Assert.Throws<ArgumentNullException>(
                () => new MustNotBeUnverifiedEmailUserName<object>(queries.Object, null as Func<object, int>));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("userId");
        }

        [Theory]
        [InlineData("email@domain1.tld", "email@domain2.tld")]
        [InlineData("email1@domain.tld", "email2@domain.tld")]
        [InlineData("email@domain.com", "email@domain.net")]
        public void IsInvalid_WhenEmailAddressByUserId_IsFound_CaseInsensitively(string userName, string emailValue)
        {
            var userId = FakeData.Id();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            EmailAddress emailAddress = new ProxiedEmailAddress(FakeData.Id())
            {
                UserId = userId,
                IsVerified = true,
                Value = emailValue,
            };
            var data = new[] { emailAddress }.AsQueryable();
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            Expression<Func<EmailAddressesBy, bool>> expectedQuery = x => x.UserId == userId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(entitySet.AsQueryable()));
            var command = new FakeMustNotBeUnverifiedEmailUserNameCommand
            {
                UserName = userName,
                UserId = userId,
            };
            var validator = new FakeMustNotBeUnverifiedEmailUserNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> ticketError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(ticketError).ShouldEqual(1);
            result.Errors.Single(ticketError).ErrorMessage.ShouldEqual(Resources
                .Validation_UserName_AllowedEmailAddress
                .Replace("{PropertyName}", User.Constraints.NameLabel.ToLower())
                .Replace("{PropertyValue}", userName)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public void IsValid_WhenUserName_IsEmpty_WithUserId(string userName)
        {
            var userId = FakeData.Id();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBeUnverifiedEmailUserNameCommand
            {
                UserName = userName,
                UserId = userId,
            };
            var validator = new FakeMustNotBeUnverifiedEmailUserNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
        }

        [Theory]
        [InlineData("username@@domain.tld")]
        [InlineData("username@domain..tld")]
        [InlineData("username@domaintld")]
        public void IsValid_WhenUserName_DoesNotMatchEmailRegEx_WithUserId(string userName)
        {
            var userId = FakeData.Id();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotBeUnverifiedEmailUserNameCommand
            {
                UserName = userName,
                UserId = userId,
            };
            var validator = new FakeMustNotBeUnverifiedEmailUserNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            queries.Verify(x => x.Execute(It.IsAny<EmailAddressesBy>()), Times.Never);
        }

        [Theory]
        [InlineData("email1@domain.tld", "Email1@Domain.Tld")]
        [InlineData("email2@domain.tld", "email2@domain.tld")]
        [InlineData("Email3@Domain.Tld", "email3@domain.tld")]
        public void IsValid_WhenEmailAddressByUserId_IsFound_CaseInsensitively(string userName, string emailValue)
        {
            var userId = FakeData.Id();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            EmailAddress emailAddress = new ProxiedEmailAddress(FakeData.Id())
            {
                UserId = userId,
                IsVerified = true,
                Value = emailValue,
            };
            var data = new[] { emailAddress }.AsQueryable();
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            Expression<Func<EmailAddressesBy, bool>> expectedQuery = x => x.UserId == userId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(entitySet.AsQueryable()));
            var command = new FakeMustNotBeUnverifiedEmailUserNameCommand
            {
                UserName = userName,
                UserId = userId,
            };
            var validator = new FakeMustNotBeUnverifiedEmailUserNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.IsAny<EmailVerificationBy>()), Times.Never);
        }

        #endregion
    }

    public class FakeMustNotBeUnverifiedEmailUserNameCommand
    {
        public string UserName { get; set; }
        public string Ticket { get; set; }
        public int? UserId { get; set; }
    }

    public class FakeMustNotBeUnverifiedEmailUserNameValidator : AbstractValidator<FakeMustNotBeUnverifiedEmailUserNameCommand>
    {
        public FakeMustNotBeUnverifiedEmailUserNameValidator(IProcessQueries queries)
        {
            When(x => x.UserId.HasValue, () =>
                RuleFor(x => x.UserName)
                    .MustNotBeUnverifiedEmailUserName(queries, x =>
                    {
                        Debug.Assert(x.UserId.HasValue);
                        return x.UserId.Value;
                    })
                    .WithName(EmailVerification.Constraints.Label)
            );

            When(x => !x.UserId.HasValue, () =>
                RuleFor(x => x.UserName)
                    .MustNotBeUnverifiedEmailUserName(queries, x => x.Ticket)
                    .WithName(EmailVerification.Constraints.Label)
            );
        }
    }
}
