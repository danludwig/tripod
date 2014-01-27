using System;
using System.Security.Cryptography.X509Certificates;
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
    }

    public class ValidateChangeUserNameCommand : AbstractValidator<ChangeUserName>
    {
        public ValidateChangeUserNameCommand(IProcessQueries queries)
        {
            RuleFor(x => x.UserId)
                .MustFindUserById(queries)
                    .WithName(User.Constraints.Label);

            RuleFor(x => x.Principal)
                .NotNull()
                .MustFindUserByPrincipal(queries)
                .MustBePrincipalWithUserId(queries, x => x.UserId)
                    .WithName(User.Constraints.Label);

            RuleFor(x => x.UserName)
                .MustBeValidUserName()
                .MustNotFindUserByName(queries, x => x.UserId)
                    .WithName(User.Constraints.NameLabel);
        }
    }

    public class HandleChangeUserNameCommand : IHandleCommand<ChangeUserName>
    {
        private readonly IWriteEntities _entities;

        public HandleChangeUserNameCommand(IWriteEntities entities)
        {
            _entities = entities;
        }

        public async Task Handle(ChangeUserName command)
        {
            //var entity = new User
            //{
            //    Name = command.Name,
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //};
            //_entities.Create(entity);

            //command.Created = entity;
            //return Task.FromResult(0);
        }
    }
}
