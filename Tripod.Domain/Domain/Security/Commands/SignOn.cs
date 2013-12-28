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
        public UserLoginInfo UserLoginInfo { get; set; }
        public bool IsPersistent { get; set; }
    }

    public class ValidateSignOnCommand : AbstractValidator<SignOn>
    {
        public ValidateSignOnCommand(IProcessQueries queries)
        {
            RuleFor(x => x.UserLoginInfo)
                .NotNull().WithName(RemoteMembership.Constraints.Label)
                .MustFindUserByLoginInfo(queries);
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
            var user = await _entities.Query<User>().ByUserLoginInfoAsync(command.UserLoginInfo, false);
            await _authenticator.SignOn(user, command.IsPersistent);
        }
    }
}
