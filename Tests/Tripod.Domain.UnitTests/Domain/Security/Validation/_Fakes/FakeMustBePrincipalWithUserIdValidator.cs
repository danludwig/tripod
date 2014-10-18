using System.Security.Principal;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeMustBePrincipalWithUserIdCommand
    {
        public IPrincipal Principal { get; set; }
        public int UserId { get; set; }
    }

    public class FakeMustBePrincipalWithUserIdValidator : AbstractValidator<FakeMustBePrincipalWithUserIdCommand>
    {
        public FakeMustBePrincipalWithUserIdValidator()
        {
            RuleFor(x => x.Principal)
                .MustBePrincipalWithUserId(x => x.UserId)
                .WithName(User.Constraints.Label)
            ;
        }
    }
}