using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Authenticate user's remote membership.
    /// </summary>
    public class SignOn : IDefineCommand
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public bool IsPersistent { get; [UsedImplicitly] set; }
    }

    public class ValidateSignOnCommand : AbstractValidator<SignOn>
    {
        public ValidateSignOnCommand(IProcessQueries queries)
        {
            RuleFor(x => x.LoginProvider)
                .NotEmpty().WithName(RemoteMembership.Constraints.ProviderLabel)
            ;

            RuleFor(x => x.ProviderKey)
                .NotEmpty()
                .MustFindUserByLoginProviderKey(queries, x => x.LoginProvider)
                    .WithName(RemoteMembership.Constraints.ProviderUserIdLabel)
            ;
        }
    }

    public class HandleSignOnCommand : IHandleCommand<SignOn>
    {
        private readonly IReadEntities _entities;
        private readonly IAuthenticate _authenticator;

        public HandleSignOnCommand(IReadEntities entities, IAuthenticate authenticator)
        {
            _entities = entities;
            _authenticator = authenticator;
        }

        public async Task Handle(SignOn command)
        {
            var userLoginInfo = new UserLoginInfo(command.LoginProvider, command.ProviderKey);
            var user = await _entities.Query<User>()
                .ByUserLoginInfoAsync(userLoginInfo, false);
            await _authenticator.SignOn(user, command.IsPersistent);
        }
    }
}
