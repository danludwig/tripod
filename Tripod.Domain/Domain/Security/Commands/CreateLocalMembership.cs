using System;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class CreateLocalMembership : BaseCreateEntityCommand<LocalMembership>, IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
        public string UserName { get; [UsedImplicitly] set; }
        public string Password { get; [UsedImplicitly] set; }
        public string ConfirmPassword { get; [UsedImplicitly] set; }
        public string Token { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class ValidateCreateLocalMembershipCommand : AbstractValidator<CreateLocalMembership>
    {
        public ValidateCreateLocalMembershipCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
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
                .MustBeRedeemableVerifyEmailToken(queries)
                .MustBePurposedVerifyEmailToken(queries, x => EmailVerificationPurpose.CreateLocalUser)
                .WithName(EmailVerification.Constraints.Label)
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
            var hasUserId = command.Principal != null && command.Principal.Identity.HasAppUserId();
            var user = hasUserId ? await _entities.GetAsync<User>(command.Principal.Identity.GetAppUserId()) : null;
            if (!hasUserId)
            {
                var createUser = new CreateUser { Name = command.UserName };
                await _commands.Execute(createUser);
                user = createUser.CreatedEntity;

                // verify & associate email address
                await _commands.Execute(new RedeemEmailVerification(user)
                {
                    Commit = false,
                    Token = command.Token,
                });
            }

            user.LocalMembership = new LocalMembership
            {
                Owner = user,
                PasswordHash = await _queries.Execute(new HashedPassword(command.Password)),
            };
            user.SecurityStamp = Guid.NewGuid().ToString();
            await _entities.SaveChangesAsync();
            command.CreatedEntity = user.LocalMembership;
        }
    }
}
