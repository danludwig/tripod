using System;
using System.Globalization;
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

namespace Tripod.Domain.Security
{
    public class DeleteEmailAddressTests : FluentValidationTests
    {
        [Fact]
        public void Validator_Principal_IsInvalid_WhenNull()
        {
            var command = new DeleteEmailAddress();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateDeleteEmailAddressCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_DoesNotExist
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", "")
            );
        }

        [Fact]
        public void Validator_EmailAddressId_IsInvalid_WhenUserId_DoesNotEqualPrincipalAppUserId()
        {
            var command = new DeleteEmailAddress
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.Id().ToString(CultureInfo.InvariantCulture)), 
                }, "authenticationType"), null),
                EmailAddressId = FakeData.Id(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<EmailAddressBy, bool>> expectedEmailAddressByQuery =
                x => x.Id == command.EmailAddressId;
            queries.Setup(x => x.Execute(It.IsAny<UserBy>())).Returns(Task.FromResult(null as User));
            queries.Setup(x => x.Execute(It.Is(expectedEmailAddressByQuery)))
                .Returns(Task.FromResult(new EmailAddress
                {
                    User = new ProxiedUser(FakeData.Id(canNotBe: command.Principal.Identity.GetUserId<int>())),
                }));
            var validator = new ValidateDeleteEmailAddressCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.EmailAddressId);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_NotAuthorized_IntIdValue
                .Replace("{PropertyName}", EmailAddress.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", command.EmailAddressId.ToString(CultureInfo.InvariantCulture))
            );
            queries.Verify(x => x.Execute(It.Is(expectedEmailAddressByQuery)), Times.AtLeast(1));
        }

        [Fact]
        public void Handler_LoadsEmailAddress_ById()
        {
            var command = new DeleteEmailAddress();
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            entities.Setup(x => x.GetAsync<EmailAddress>(command.EmailAddressId))
                .Returns(Task.FromResult(new EmailAddress()));
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleDeleteEmailAddressCommand(entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.GetAsync<EmailAddress>(command.EmailAddressId), Times.Once);
        }

        [Fact]
        public void Handler_DeletesEmailAddress_ById_WhenIsVerified()
        {
            var command = new DeleteEmailAddress
            {
                EmailAddressId = FakeData.Id(),
            };
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            EmailAddress emailAddress = new ProxiedEmailAddress(command.EmailAddressId)
            {
                IsVerified = true,
            };
            entities.Setup(x => x.GetAsync<EmailAddress>(command.EmailAddressId))
                .Returns(Task.FromResult(emailAddress));
            Expression<Func<EmailAddress, bool>> expectedEmailAddressToDelete =
                x => x.Id == command.EmailAddressId;
            entities.Setup(x => x.Delete(It.Is(expectedEmailAddressToDelete)));
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleDeleteEmailAddressCommand(entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Delete(It.Is(expectedEmailAddressToDelete)), Times.Once);
            entities.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
