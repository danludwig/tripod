using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class DeleteRemoteMembership : IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }

    public class ValidateDeleteRemoteMembershipCommand : AbstractValidator<DeleteRemoteMembership>
    {
        public ValidateDeleteRemoteMembershipCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .NotNull().WithName(User.Constraints.Label)
                .MustFindUserByPrincipal(queries)

                // to delete this remote membership, user must either have a local password or a different external login
                .MustFindLocalMembershipByPrincipal(queries)
                    .When(x => !queries.Execute(new RemoteMembershipsByUser(int.Parse(x.Principal.Identity.GetUserId())))
                        .Result.Any(y => y.LoginProvider != x.LoginProvider && y.ProviderKey != x.ProviderKey),
                            ApplyConditionTo.CurrentValidator)
                    .WithMessage(Resources.Validation_RemoteMembershipByUser_IsOnlyLogin)
            ;

            RuleFor(x => x.LoginProvider)
                .NotEmpty()
                .WithName(RemoteMembership.Constraints.ProviderLabel);

            RuleFor(x => x.ProviderKey)
                .NotEmpty()
                .WithName(RemoteMembership.Constraints.ProviderUserIdLabel);
        }
    }

    public class HandleDeleteRemoteMembershipCommand : IHandleCommand<DeleteRemoteMembership>
    {
        private readonly IWriteEntities _entities;

        public HandleDeleteRemoteMembershipCommand(IWriteEntities entities)
        {
            _entities = entities;
        }

        public async Task Handle(DeleteRemoteMembership command)
        {
            var userId = command.Principal.Identity.GetUserId();
            var userLoginInfo = new UserLoginInfo(command.LoginProvider, command.ProviderKey);
            var remoteMembership = await _entities.Get<RemoteMembership>()
                .EagerLoad(new Expression<Func<RemoteMembership, object>>[] { x => x.Owner, })
                .ByUserIdAndLoginInfoAsync(int.Parse(userId), userLoginInfo);
            if (remoteMembership == null) return;

            _entities.Delete(remoteMembership);
            await _entities.SaveChangesAsync();
        }
    }
}
