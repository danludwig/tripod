using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class MustFindRemoteMembershipTicketTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustFindRemoteMembershipTicket(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenRemoteMembershipTicket_IsNotFound()
        {
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindRemoteMembershipTicketCommand { Principal = principal.Object };
            Expression<Func<PrincipalRemoteMembershipTicket, bool>> expectedQuery =
                x => x.Principal == principal.Object;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(null as RemoteMembershipTicket));
            var validator = new FakeMustFindRemoteMembershipTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> principalError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(principalError).ShouldEqual(1);
            result.Errors.Single(principalError).ErrorMessage.ShouldEqual(
                Resources.Validation_RemoteMembership_NoTicket
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenRemoteMembershipTicket_IsFound()
        {
            var principal = new Mock<IPrincipal>(MockBehavior.Strict);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindRemoteMembershipTicketCommand { Principal = principal.Object };
            var remoteMembershipTicket = new RemoteMembershipTicket();
            Expression<Func<PrincipalRemoteMembershipTicket, bool>> expectedQuery =
                x => x.Principal == principal.Object;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(remoteMembershipTicket));
            var validator = new FakeMustFindRemoteMembershipTicketValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.Principal, command.Principal);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustFindRemoteMembershipTicketCommand
    {
        public IPrincipal Principal { get; set; }
    }

    public class FakeMustFindRemoteMembershipTicketValidator : AbstractValidator<FakeMustFindRemoteMembershipTicketCommand>
    {
        public FakeMustFindRemoteMembershipTicketValidator(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .MustFindRemoteMembershipTicket(queries)
                .WithName(User.Constraints.Label)
            ;
        }
    }
}
