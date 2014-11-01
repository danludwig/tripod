using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class MustNotFindRemoteMembershipTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustNotFindRemoteMembership(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenRemoteMembershipTicket_IsFound_AndEntity_BelongsToDifferentPrincipal()
        {
            int userId = FakeData.Id();
            string loginProvider = FakeData.String();
            string providerKey = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindRemoteMembershipCommand
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
                }, "authenticationType"), null),
            };
            Expression<Func<PrincipalRemoteMembershipTicket, bool>> expectedTicketQuery =
                x => x.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedTicketQuery)))
                .Returns(Task.FromResult(new RemoteMembershipTicket
                {
                    Login = new UserLoginInfo(loginProvider, providerKey),
                }));
            Expression<Func<RemoteMembershipBy, bool>> expectedEntityQuery =
                x => x.UserId == null && x.UserName == null &&
                    x.UserLoginInfo.LoginProvider == loginProvider &&
                    x.UserLoginInfo.ProviderKey == providerKey;
            queries.Setup(x => x.Execute(It.Is(expectedEntityQuery)))
                .Returns(Task.FromResult(new ProxiedRemoteMembership(loginProvider, providerKey)
                {
                    UserId = FakeData.Id(),
                } as RemoteMembership));
            var validator = new FakeMustNotFindRemoteMembershipValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> principalError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(principalError).ShouldEqual(1);
            result.Errors.Single(principalError).ErrorMessage.ShouldEqual(Resources
                .Validation_RemoteMembership_AlreadyAssigned
                .Replace("{ProviderName}", loginProvider)
            );
            queries.Verify(x => x.Execute(It.Is(expectedTicketQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(expectedEntityQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedTicketQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.Is(expectedEntityQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenPrincipal_IsNull()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindRemoteMembershipCommand();
            Expression<Func<PrincipalRemoteMembershipTicket, bool>> expectedTicketQuery =
                x => x.Principal == null;
            queries.Setup(x => x.Execute(It.Is(expectedTicketQuery)))
                .Returns(Task.FromResult(null as RemoteMembershipTicket));
            var validator = new FakeMustNotFindRemoteMembershipValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedTicketQuery)), Times.Never);
            queries.Verify(x => x.Execute(It.IsAny<RemoteMembershipBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedTicketQuery)), Times.Never);
            queries.Verify(x => x.Execute(It.IsAny<RemoteMembershipBy>()), Times.Never);
        }

        [Fact]
        public void IsValid_WhenRemoteMembershipTicket_IsNotFound()
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindRemoteMembershipCommand
            {
                Principal = new GenericPrincipal(new GenericIdentity(FakeData.String()), null),
            };
            Expression<Func<PrincipalRemoteMembershipTicket, bool>> expectedTicketQuery =
                x => x.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedTicketQuery)))
                .Returns(Task.FromResult(null as RemoteMembershipTicket));
            var validator = new FakeMustNotFindRemoteMembershipValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedTicketQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.IsAny<RemoteMembershipBy>()), Times.Never);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedTicketQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.IsAny<RemoteMembershipBy>()), Times.Never);
        }

        [Fact]
        public void IsValid_WhenRemoteMembershipTicket_IsFound_AndEntity_IsNotFound()
        {
            string loginProvider = FakeData.String();
            string providerKey = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindRemoteMembershipCommand
            {
                Principal = new GenericPrincipal(new GenericIdentity(FakeData.String()), null),
            };
            Expression<Func<PrincipalRemoteMembershipTicket, bool>> expectedTicketQuery =
                x => x.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedTicketQuery)))
                .Returns(Task.FromResult(new RemoteMembershipTicket
                {
                    Login = new UserLoginInfo(loginProvider, providerKey),
                }));
            Expression<Func<RemoteMembershipBy, bool>> expectedEntityQuery =
                x => x.UserId == null && x.UserName == null &&
                    x.UserLoginInfo.LoginProvider == loginProvider &&
                    x.UserLoginInfo.ProviderKey == providerKey;
            queries.Setup(x => x.Execute(It.Is(expectedEntityQuery)))
                .Returns(Task.FromResult(null as RemoteMembership));
            var validator = new FakeMustNotFindRemoteMembershipValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedTicketQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(expectedEntityQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedTicketQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.Is(expectedEntityQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenRemoteMembershipTicket_IsFound_AndEntity_BelongsToPrincipal()
        {
            int userId = FakeData.Id();
            string loginProvider = FakeData.String();
            string providerKey = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindRemoteMembershipCommand
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
                }, "authenticationType"), null),
            };
            Expression<Func<PrincipalRemoteMembershipTicket, bool>> expectedTicketQuery =
                x => x.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedTicketQuery)))
                .Returns(Task.FromResult(new RemoteMembershipTicket
                {
                    Login = new UserLoginInfo(loginProvider, providerKey),
                }));
            Expression<Func<RemoteMembershipBy, bool>> expectedEntityQuery =
                x => x.UserId == null && x.UserName == null &&
                    x.UserLoginInfo.LoginProvider == loginProvider &&
                    x.UserLoginInfo.ProviderKey == providerKey;
            queries.Setup(x => x.Execute(It.Is(expectedEntityQuery)))
                .Returns(Task.FromResult(new ProxiedRemoteMembership(loginProvider, providerKey)
                {
                    UserId = userId,
                } as RemoteMembership));
            var validator = new FakeMustNotFindRemoteMembershipValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedTicketQuery)), Times.Once);
            queries.Verify(x => x.Execute(It.Is(expectedEntityQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedTicketQuery)), Times.Exactly(2));
            queries.Verify(x => x.Execute(It.Is(expectedEntityQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustNotFindRemoteMembershipCommand
    {
        public IPrincipal Principal { get; set; }
    }

    public class FakeMustNotFindRemoteMembershipValidator : AbstractValidator<FakeMustNotFindRemoteMembershipCommand>
    {
        public FakeMustNotFindRemoteMembershipValidator(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .MustNotFindRemoteMembership(queries)
                .WithName(User.Constraints.Label)
            ;
        }
    }
}
