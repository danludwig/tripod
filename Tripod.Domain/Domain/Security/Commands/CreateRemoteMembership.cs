using System;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class CreateRemoteMembership : IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
        public string UserName { get; set; }
        public RemoteMembership Created { get; internal set; }
    }

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
                    .WithName(User.Constraints.NameLabel)
                    .When(x => !x.Principal.Identity.IsAuthenticated)
            ;
        }
    }

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
            await _entities.SaveChangesAsync();
            command.Created = entity;
        }
    }
}
