using System;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class CreateLocalMembership : BaseCreateEntityCommand<LocalMembership>, IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
        public string UserName { get; [UsedImplicitly] set; }
        public string Password { get; [UsedImplicitly] set; }
        public string ConfirmPassword { get; [UsedImplicitly] set; }
        public string Token { get; [UsedImplicitly] set; }
        public string Ticket { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class ValidateCreateLocalMembershipCommand : AbstractValidator<CreateLocalMembership>
    {
        public ValidateCreateLocalMembershipCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .MustFindUserByPrincipal(queries)
                .MustNotFindLocalMembershipByPrincipal(queries)
                .When(x => x.Principal.Identity.IsAuthenticated)
                .WithName(User.Constraints.Label)
            ;

            RuleFor(x => x.UserName)
                .MustBeValidUserName()
                .MustNotFindUserByName(queries)
                .MustNotBeUnverifiedEmailUserName(queries, x => x.Ticket)
                .When(x => !x.Principal.Identity.IsAuthenticated)
                .WithName(User.Constraints.NameLabel)
            ;

            RuleFor(x => x.Password)
                .MustBeValidPassword()
                .WithName(LocalMembership.Constraints.PasswordLabel)
            ;

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .MustEqualPassword(x => x.Password)
                    .When(x => !string.IsNullOrWhiteSpace(x.Password),
                        ApplyConditionTo.CurrentValidator)
                .WithName(LocalMembership.Constraints.PasswordConfirmationLabel)
            ;

            RuleFor(x => x.Ticket)
                .MustBeRedeemableVerifyEmailTicket(queries)
                .MustBePurposedVerifyEmailTicket(queries, x => EmailVerificationPurpose.CreateLocalUser)
                .MustHaveValidVerifyEmailToken(queries, x => x.Token)

                // ticket is not required when signed-on user creates a local password
                .When(x => !x.Principal.Identity.IsAuthenticated)
                .WithName(EmailVerification.Constraints.Label)
            ;
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
            var user = hasUserId ? await _entities.GetAsync<User>(command.Principal.Identity.GetUserId<int>()) : null;
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
                    Ticket = command.Ticket,
                });
            }

            user.LocalMembership = new LocalMembership
            {
                User = user,
                PasswordHash = await _queries.Execute(new HashedPassword(command.Password)),
            };
            user.SecurityStamp = Guid.NewGuid().ToString();
            await _entities.SaveChangesAsync();
            command.CreatedEntity = user.LocalMembership;
        }
    }
}
