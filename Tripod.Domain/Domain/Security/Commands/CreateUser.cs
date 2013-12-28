using System;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class CreateUser : IDefineCommand
    {
        public CreateUser() { }
        public string Name { get; set; }
        public User Created { get; internal set; }
    }

    public class ValidateUserName : AbstractValidator<string>
    {
        public ValidateUserName()
        {
            // username is required, has min/max lengths, and cannot already exist
            RuleFor(x => x)
                .NotEmpty().WithName(User.Constraints.NameLabel)
                .MinLength(User.Constraints.NameMinLength)
                .MaxLength(User.Constraints.NameMaxLength)
            ;
        }
    }

    public class ValidateCreateUserCommand : AbstractValidator<CreateUser>
    {
        public ValidateCreateUserCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Name).SetValidator(new ValidateUserName()).MustNotFindUserByName(queries);
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
