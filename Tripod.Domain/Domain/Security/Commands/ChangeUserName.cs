using System;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class ChangeUserName : IDefineSecuredCommand
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public IPrincipal Principal { get; set; }
        public User SignedIn { get; internal set; }
    }

    [UsedImplicitly]
    public class ValidateChangeUserNameCommand : AbstractValidator<ChangeUserName>
    {
        public ValidateChangeUserNameCommand(IProcessQueries queries)
        {
            RuleFor(x => x.UserId)
                .MustFindUserById(queries)
                .WithName(User.Constraints.Label);

            RuleFor(x => x.Principal)
                .MustFindUserByPrincipal(queries)
                .MustBePrincipalWithUserId(x => x.UserId)
                .WithName(User.Constraints.Label);

            RuleFor(x => x.UserName)
                .MustBeValidUserName()
                .MustNotBeUnverifiedEmailUserName(queries, x => x.UserId)
                .MustNotFindUserByName(queries, x => x.UserId)
                .WithName(User.Constraints.NameLabel);
        }
    }

    [UsedImplicitly]
    public class HandleChangeUserNameCommand : IHandleCommand<ChangeUserName>
    {
        private readonly IWriteEntities _entities;
        private readonly IAuthenticate _authenticator;

        public HandleChangeUserNameCommand(IWriteEntities entities, IAuthenticate authenticator)
        {
            _entities = entities;
            _authenticator = authenticator;
        }

        public async Task Handle(ChangeUserName command)
        {
            var entity = await _entities.GetAsync<User>(command.UserId);
            entity.Name = command.UserName;
            entity.SecurityStamp = Guid.NewGuid().ToString();
            await _entities.SaveChangesAsync();
            await _authenticator.SignOut();
            await _authenticator.SignOn(entity);
            command.SignedIn = entity;
        }
    }
}
