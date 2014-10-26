using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class UpdateEmailAddressTests : FluentValidationTests
    {
        [Fact]
        public void Validator_EmailAddressIdIsInvalid_WhenCommandIsPrimaryIsFalse_AndEmailIsPrimaryIsTrue()
        {
            var command = new UpdateEmailAddress
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.IdString()), 
                    new Claim(ClaimTypes.Name, FakeData.String()), 
                }, "authenticationType"), null),
                EmailAddressId = FakeData.Id(),
                IsPrimary = false,
            };
            EmailAddress emailAddress = new ProxiedEmailAddress(command.EmailAddressId)
            {
                UserId = command.Principal.Identity.GetUserId<int>(),
                IsPrimary = true,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.Is<EmailAddressBy>(y => y.Id == command.EmailAddressId)))
                .Returns(Task.FromResult(emailAddress));
            queries.Setup(x => x.Execute(It.Is<UserBy>(y => y.Principal == command.Principal)))
                .Returns(Task.FromResult(null as User));
            var validator = new ValidateUpdateEmailAddressCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.EmailAddressId);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_EmailAddress_CannotBePrimary
                .Replace("{PropertyName}", EmailAddress.Constraints.Label.ToLower())
            );
        }

        [Fact]
        public void Validator_EmailAddressIdIsValid_WhenAllRulesPass()
        {
            var command = new UpdateEmailAddress
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.IdString()), 
                    new Claim(ClaimTypes.Name, FakeData.String()), 
                }, "authenticationType"), null),
                EmailAddressId = FakeData.Id(),
                IsPrimary = true,
            };
            EmailAddress emailAddress = new ProxiedEmailAddress(command.EmailAddressId)
            {
                UserId = command.Principal.Identity.GetUserId<int>(),
                IsPrimary = false,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            queries.Setup(x => x.Execute(It.Is<EmailAddressBy>(y => y.Id == command.EmailAddressId)))
                .Returns(Task.FromResult(emailAddress));
            queries.Setup(x => x.Execute(It.Is<UserBy>(y => y.Principal == command.Principal)))
                .Returns(Task.FromResult(null as User));
            var validator = new ValidateUpdateEmailAddressCommand(queries.Object);

            var result = validator.Validate(command);

            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.EmailAddressId);
            result.Errors.Count(targetError).ShouldEqual(0);
        }

        [Fact]
        public void Handler_LoadsEmailAddress_ById()
        {
            var command = new UpdateEmailAddress
            {
                EmailAddressId = FakeData.Id(),
                IsPrimary = null,
            };
            EmailAddress emailAddressToUpdate = new ProxiedEmailAddress(command.EmailAddressId)
            {
                IsPrimary = false,
            };
            EmailAddress[] emailAddressData =
            {
                new ProxiedEmailAddress(FakeData.Id()),
                emailAddressToUpdate,
                new ProxiedEmailAddress(FakeData.Id()),
            };
            var emailAddressSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict)
                .SetupDataAsync(emailAddressData.AsQueryable());
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Get<EmailAddress>()).Returns(emailAddressSet.Object);
            var handler = new HandleUpdateEmailAddressCommand(entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<EmailAddress>(), Times.Once);
            entities.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public void Handler_ThrowsInvalidOperationException_WhenCommandIsPrimaryIsFalse_AndEmailIsPrimaryIsTrue()
        {
            var command = new UpdateEmailAddress
            {
                EmailAddressId = FakeData.Id(),
                IsPrimary = false,
            };
            EmailAddress emailAddressToUpdate = new ProxiedEmailAddress(command.EmailAddressId)
            {
                IsPrimary = true,
            };
            EmailAddress[] emailAddressData =
            {
                new ProxiedEmailAddress(FakeData.Id()),
                emailAddressToUpdate,
                new ProxiedEmailAddress(FakeData.Id()),
            };
            var emailAddressSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict)
                .SetupDataAsync(emailAddressData.AsQueryable());
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Get<EmailAddress>()).Returns(emailAddressSet.Object);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleUpdateEmailAddressCommand(entities.Object);

            var exception = Assert.Throws<AggregateException>(
                () => handler.Handle(command).Wait());

            exception.ShouldNotBeNull();
            exception.InnerExceptions.ShouldNotBeNull();
            exception.InnerExceptions.Count.ShouldEqual(1);
            var invalidOperation = exception.InnerExceptions.Single();
            invalidOperation.ShouldBeType<InvalidOperationException>();
        }

        [Fact]
        public void Handler_SavesChanges_WhenChangingPrimaryEmail()
        {
            var command = new UpdateEmailAddress
            {
                EmailAddressId = FakeData.Id(),
                IsPrimary = true,
            };
            User user = new ProxiedUser(FakeData.Id());
            EmailAddress emailAddressToUpdate = new ProxiedEmailAddress(command.EmailAddressId)
            {
                IsPrimary = false,
                User = user,
                UserId = user.Id,
            };
            EmailAddress primaryEmailAddress = new ProxiedEmailAddress(FakeData.Id())
            {
                IsPrimary = true,
                User = user,
                UserId = user.Id,
            };
            user.EmailAddresses.Add(emailAddressToUpdate);
            user.EmailAddresses.Add(primaryEmailAddress);
            EmailAddress[] emailAddressData =
            {
                new ProxiedEmailAddress(FakeData.Id()),
                emailAddressToUpdate,
                primaryEmailAddress,
                new ProxiedEmailAddress(FakeData.Id()),
            };
            var emailAddressSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict)
                .SetupDataAsync(emailAddressData.AsQueryable());
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.Get<EmailAddress>()).Returns(emailAddressSet.Object);
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleUpdateEmailAddressCommand(entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
