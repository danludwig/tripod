using System;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class CreateLocalMembership : IDefineSecuredCommand
    {
        //public CreateLocalMembership(string userName)
        //{
        //    UserName = userName;
        //}

        //public CreateLocalMembership(IPrincipal principal)
        //{
        //    if (principal == null) throw new ArgumentNullException("principal");
        //    Principal = principal;
        //}

        public IPrincipal Principal { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public LocalMembership Created { get; internal set; }
    }

    public class ValidateCreateLocalMembershipCommand : AbstractValidator<CreateLocalMembership>
    {
        public ValidateCreateLocalMembershipCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Principal).MustFindUserByPrincipal(queries).WithName(User.Constraints.Label)

                // use a different name here because we want the error message to say something like
                // "Password for user 'username' already exists."
                .MustNotFindLocalMembershipByPrincipal(queries).WithName(LocalMembership.Constraints.PasswordLabel)
                .When(x => x.Principal.Identity.GetUserId() != null)
            ;

            RuleFor(x => x.UserName)
                .MustBeValidUserName().WithName(User.Constraints.NameLabel)
                .MustNotFindUserByName(queries)
                .When(x => x.Principal.Identity.GetUserId() == null)
            ;

            RuleFor(x => x.Password)
                .MustBeValidPassword().WithName(LocalMembership.Constraints.PasswordLabel)
            ;

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithName(LocalMembership.Constraints.PasswordConfirmationLabel)
                .MustEqualPassword(x => x.Password)
                .When(x => !string.IsNullOrWhiteSpace(x.Password), ApplyConditionTo.CurrentValidator);
        }
    }

    public class HandleCreateLocalMembershipCommand : IHandleCommand<CreateLocalMembership>
    {
        private readonly IProcessCommands _commands;
        private readonly IWriteEntities _entities;
        private readonly UserManager<User, int> _userManager;

        public HandleCreateLocalMembershipCommand(IProcessCommands commands, IWriteEntities entities, UserManager<User, int> userManager)
        {
            _commands = commands;
            _entities = entities;
            _userManager = userManager;
        }

        public async Task Handle(CreateLocalMembership command)
        {
            // when user does not exist, must create both user and local membership
            if (string.IsNullOrWhiteSpace(command.Principal.Identity.Name))
            {
                var createUser = new CreateUser { Name = command.UserName };
                await _commands.Execute(createUser);
                createUser.Created.LocalMembership = new LocalMembership
                {
                    Owner = createUser.Created,
                    PasswordHash = _userManager.PasswordHasher.HashPassword(command.Password),
                    IsConfirmed = true,
                };
                _entities.SaveChanges();
                command.Created = createUser.Created.LocalMembership;
                return;
            }

            // does user already exist?
            var user = command.Principal != null
                ? await _entities.GetAsync<User>(command.Principal.Identity.GetUserId()) : null;
            if (user == null)
            {
                var createUser = new CreateUser { Name = command.UserName };
                await _commands.Execute(createUser);
                user = createUser.Created;
            }

        }
    }
}
