using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class CreateRemoteMembership : BaseCreateEntityCommand<RemoteMembership>, IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
        public string UserName { get; [UsedImplicitly] set; }
        public string Token { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class ValidateCreateRemoteMembershipCommand : AbstractValidator<CreateRemoteMembership>
    {
        public ValidateCreateRemoteMembershipCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                .NotNull()
                .MustFindUserByPrincipal(queries)
                    .When(x => x.Principal.Identity.IsAuthenticated, ApplyConditionTo.CurrentValidator)
                .MustFindRemoteMembershipTicket(queries)
                    .WithName(User.Constraints.Label)
            ;

            RuleFor(x => x.UserName)
                .MustBeValidUserName()
                .MustNotFindUserByName(queries)
                .MustNotBeUnverifiedEmailUserName(x => x.Token, queries)
                    .WithName(User.Constraints.NameLabel)
                    .When(x => !x.Principal.Identity.IsAuthenticated)
            ;

            RuleFor(x => x.Token)
                .NotEmpty()
                .MustBeValidConfirmEmailToken(queries)
                .MustFindEmailConfirmationByToken(queries)
                .MustNotBeRedeemedConfirmEmailToken(queries)
                .MustNotBeExpiredConfirmEmailToken(queries)
                .MustBePurposedConfirmEmailToken(x => EmailConfirmationPurpose.CreateRemoteUser, queries)
                .WithName(EmailConfirmation.Constraints.Label)
                    .When(x => !x.Principal.Identity.IsAuthenticated);
        }
    }

    [UsedImplicitly]
    public class HandleCreateRemoteMembershipCommand : IHandleCommand<CreateRemoteMembership>
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;
        private readonly IWriteEntities _entities;

        public HandleCreateRemoteMembershipCommand(IProcessQueries queries, IProcessCommands commands, IWriteEntities entities)
        {
            _commands = commands;
            _entities = entities;
            _queries = queries;
        }

        public async Task Handle(CreateRemoteMembership command)
        {
            // does user already exist?
            var userId = command.Principal.Identity.GetUserId();
            var user = userId != null ? await _entities.Get<User>()
                .EagerLoad(new Expression<Func<User, object>>[]
                {
                    x => x.RemoteMemberships,
                })
                .ByIdAsync(int.Parse(userId)) : null;
            if (userId == null)
            {
                var createUser = new CreateUser { Name = command.UserName };
                await _commands.Execute(createUser);
                user = createUser.Created;

                // confirm & associate email address
                //userToken = _userManager.UserConfirmationTokens.Validate(command.Token);
                var userToken = await _queries.Execute(new EmailConfirmationUserToken(command.Token));
                var confirmation = await _entities.Get<EmailConfirmation>()
                    .EagerLoad(x => x.Owner)
                    .ByTicketAsync(userToken.Value, false);
                var email = confirmation.Owner;
                confirmation.RedeemedOnUtc = DateTime.UtcNow;
                email.IsConfirmed = true;
                email.IsDefault = true;
                email.Owner = user;

                // expire unused confirmations
                var unusedConfirmations = await _entities.Get<EmailConfirmation>()
                    .ByOwnerValue(email.Value)
                    .ToArrayAsync()
                ;
                foreach (var unusedConfirmation in unusedConfirmations.Except(new[] { confirmation }))
                    unusedConfirmation.RedeemedOnUtc = unusedConfirmation.ExpiresOnUtc;
            }

            var ticket = await _queries.Execute(new PrincipalRemoteMembershipTicket(command.Principal));

            // do not add this login if it already exists
            if (user.RemoteMemberships.ByUserLoginInfo(ticket.Login) != null) return;

            var entity = new RemoteMembership
            {
                Owner = user,
                UserId = user.Id,
                Id =
                {
                    LoginProvider = ticket.Login.LoginProvider,
                    ProviderKey = ticket.Login.ProviderKey
                },
            };
            user.RemoteMemberships.Add(entity);
            user.SecurityStamp = Guid.NewGuid().ToString();

            if (command.Commit) await _entities.SaveChangesAsync();
            command.CreatedEntity = entity;
        }
    }
}
