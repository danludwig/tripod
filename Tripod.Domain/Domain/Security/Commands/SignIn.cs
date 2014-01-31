using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Authenticate user's local membership.
    /// </summary>
    public class SignIn : IDefineCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsPersistent { get; set; }
        public User SignedIn { get; internal set; }
    }

    public class ValidateSignInCommand : AbstractValidator<SignIn>
    {
        public ValidateSignInCommand(IProcessQueries queries)
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MustFindUserByNameOrEmail(queries)
                    .WithName(string.Format("{0} or {1}", User.Constraints.NameLabel, EmailAddress.Constraints.Label))
            ;
            
            RuleFor(x => x.Password)
                .NotEmpty()
                .MustBeVerifiedPassword(queries, x => x.UserName)
                    .WithName(LocalMembership.Constraints.PasswordLabel)
                    .When(x => queries.Execute(new UserBy(x.UserName)).Result != null, ApplyConditionTo.CurrentValidator)
            ;
        }
    }

    public class HandleSignInCommand : IHandleCommand<SignIn>
    {
        private readonly IProcessQueries _queries;
        private readonly UserManager<User, int> _userManager;
        private readonly IAuthenticate _authenticator;

        public HandleSignInCommand(IProcessQueries queries, UserManager<User, int> userManager, IAuthenticate authenticator)
        {
            _queries = queries;
            _userManager = userManager;
            _authenticator = authenticator;
        }

        public async Task Handle(SignIn command)
        {
            // match password with either a username or a verified email address
            var user = await _queries.Execute(new UserByNameOrVerifiedEmail(command.UserName));
            if (user == null) return;

            user = await _userManager.FindAsync(user.Name, command.Password);
            await _authenticator.SignOn(user, command.IsPersistent);
            command.SignedIn = user;
        }
    }
}
