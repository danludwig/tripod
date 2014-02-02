using System;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class ChangeLocalPassword : IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
        public string OldPassword { get; [UsedImplicitly] set; }
        public string NewPassword { get; [UsedImplicitly] set; }
        public string ConfirmPassword { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class ValidateChangeLocalPasswordCommand : AbstractValidator<ChangeLocalPassword>
    {
        public ValidateChangeLocalPasswordCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .MustFindUserByPrincipal(queries)
                .MustFindLocalMembershipByPrincipal(queries)
                    .WithName(User.Constraints.Label);

            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .MustBeVerifiedPassword(queries, x => x.Principal.Identity.Name)
                    .WithMessage(Resources.Validation_InvalidPassword)
                    .WithName(LocalMembership.Constraints.OldPasswordLabel);

            RuleFor(x => x.NewPassword)
                .MustBeValidPassword()
                    .WithName(LocalMembership.Constraints.NewPasswordLabel)
            ;

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .MustEqualPassword(x => x.NewPassword, LocalMembership.Constraints.NewPasswordLabel)
                    .WithName(LocalMembership.Constraints.NewPasswordConfirmationLabel)
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword), ApplyConditionTo.CurrentValidator);
        }
    }

    [UsedImplicitly]
    public class HandleChangeLocalPasswordCommand : IHandleCommand<ChangeLocalPassword>
    {
        private readonly IWriteEntities _entities;
        private readonly UserManager<User, int> _userManager;

        public HandleChangeLocalPasswordCommand(IWriteEntities entities, UserManager<User, int> userManager)
        {
            _entities = entities;
            _userManager = userManager;
        }

        public async Task Handle(ChangeLocalPassword command)
        {
            var userId = command.Principal.Identity.GetAppUserId();
            var user = await _entities.Get<User>()
                .EagerLoad(new Expression<Func<User, object>>[] { x => x.LocalMembership, })
                .ByIdAsync(userId);

            user.LocalMembership.PasswordHash = _userManager.PasswordHasher.HashPassword(command.NewPassword);
            user.SecurityStamp = Guid.NewGuid().ToString();
            await _entities.SaveChangesAsync();
        }
    }
}
