using System;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class CreateUser : IDefineCommand
    {
        internal CreateUser() { }
        internal string Name { get; set; }
        internal User Created { get; set; }
    }

    public class ValidateCreateUserCommand : AbstractValidator<CreateUser>
    {
        public ValidateCreateUserCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Name)
                .MustBeValidUserName().WithName(User.Constraints.NameLabel)
                .MustNotFindUserByName(queries)
            ;
        }
    }

    public class HandleCreateUserCommand : IHandleCommand<CreateUser>
    {
        private readonly IWriteEntities _entities;

        public HandleCreateUserCommand(IWriteEntities entities)
        {
            _entities = entities;
        }

        public Task Handle(CreateUser command)
        {
            var entity = new User
            {
                Name = command.Name,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            _entities.Create(entity);

            command.Created = entity;
            return Task.FromResult(0);
        }
    }
}
