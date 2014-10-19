using System.Security.Principal;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeMustBeUnauthenticatedPrincipalCommand
    {
        public IPrincipal Principal { get; set; }
    }

    public class FakeMustBeUnauthenticatedPrincipalValidator : AbstractValidator<FakeMustBeUnauthenticatedPrincipalCommand>
    {
        public FakeMustBeUnauthenticatedPrincipalValidator()
        {
            RuleFor(x => x.Principal)
                .MustBeUnauthenticatedPrincipal()
                .WithName(User.Constraints.Label)
            ;
        }
    }
}