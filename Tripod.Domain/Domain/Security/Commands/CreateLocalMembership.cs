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
                .NotEmpty()
                .MustBeValidConfirmEmailToken(queries)
                .MustFindEmailConfirmationByToken(queries)
                .MustNotBeRedeemedConfirmEmailToken(queries)
                .MustNotBeExpiredConfirmEmailToken(queries)
                .MustBePurposedConfirmEmailToken(x => EmailConfirmationPurpose.CreateLocalUser, queries)
                .WithName(EmailConfirmation.Constraints.Label)
                    .When(x => !x.Principal.Identity.IsAuthenticated);
        }
    }

    [UsedImplicitly]
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

                // confirm & associate email address
                var userToken = _userManager.UserConfirmationTokens.Validate(command.Token);
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
