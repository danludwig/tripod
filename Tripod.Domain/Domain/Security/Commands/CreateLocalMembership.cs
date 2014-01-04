using System;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class CreateLocalMembership : IDefineSecuredCommand
    {
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
            RuleFor(x => x.Principal)
                .NotNull()
                .MustFindUserByPrincipal(queries)
                .MustNotFindLocalMembershipByPrincipal(queries)
                    .WithName(User.Constraints.Label)
                    .When(x => x.Principal.Identity.IsAuthenticated)
            ;

            RuleFor(x => x.UserName)
                .MustBeValidUserName()
                .MustNotFindUserByName(queries)
                    .WithName(User.Constraints.NameLabel)
                    .When(x => !x.Principal.Identity.IsAuthenticated)
            ;

            RuleFor(x => x.Password)
                .MustBeValidPassword()
                    .WithName(LocalMembership.Constraints.PasswordLabel)
            ;

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .MustEqualPassword(x => x.Password)
                    .WithName(LocalMembership.Constraints.PasswordConfirmationLabel)
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
            // does user already exist?
            var userId = command.Principal.Identity.GetUserId();
            var user = userId != null ? await _entities.GetAsync<User>(int.Parse(userId)) : null;
            if (userId == null)
            {
                var createUser = new CreateUser { Name = command.UserName };
                await _commands.Execute(createUser);
                user = createUser.Created;
            }

            user.LocalMembership = new LocalMembership
            {
                Owner = user,
                PasswordHash = _userManager.PasswordHasher.HashPassword(command.Password),
                IsConfirmed = true,
            };
            user.SecurityStamp = Guid.NewGuid().ToString();
            await _entities.SaveChangesAsync();
            command.Created = user.LocalMembership;
        }
    }
}
