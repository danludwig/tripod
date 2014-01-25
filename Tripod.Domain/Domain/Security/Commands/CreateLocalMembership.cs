using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class CreateLocalMembership : IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
        public string UserName { get; [UsedImplicitly] set; }
        public string Password { get; [UsedImplicitly] set; }
        public string ConfirmPassword { get; [UsedImplicitly] set; }
        public string Token { get; [UsedImplicitly] set; }
        public LocalMembership Created { [UsedImplicitly] get; internal set; }
    }

    [UsedImplicitly]
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
                .MustNotBeUnverifiedEmailUserName(x => x.Token, queries)
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

            RuleFor(x => x.Token)
                .MustBeRedeemableConfirmEmailToken(queries)
                .MustBePurposedConfirmEmailToken(queries, x => EmailConfirmationPurpose.CreateLocalUser)
                .WithName(EmailConfirmation.Constraints.Label)
                    .When(x => !x.Principal.Identity.IsAuthenticated);
        }
    }

    [UsedImplicitly]
    public class HandleCreateLocalMembershipCommand : IHandleCommand<CreateLocalMembership>
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;
        private readonly IWriteEntities _entities;

        public HandleCreateLocalMembershipCommand(IProcessQueries queries, IProcessCommands commands, IWriteEntities entities)
        {
            _queries = queries;
            _commands = commands;
            _entities = entities;
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

                // confirm & associate email address
                await _commands.Execute(new RedeemEmailConfirmation(command.Token, user));
            }

            user.LocalMembership = new LocalMembership
            {
                Owner = user,
                PasswordHash = await _queries.Execute(new HashedPassword(command.Password)),
                IsConfirmed = true,
            };
            user.SecurityStamp = Guid.NewGuid().ToString();
            await _entities.SaveChangesAsync();
            command.Created = user.LocalMembership;
        }
    }
}
