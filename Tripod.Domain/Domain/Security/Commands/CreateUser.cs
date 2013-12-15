using FluentValidation;

namespace Tripod.Domain.Security
{
    public class CreateUser : IDefineCommand
    {
        public string Name { get; set; }
        public User Created { get; internal set; }
    }

    public class ValidateCreateUserCommand : AbstractValidator<CreateUser>
    {
        public ValidateCreateUserCommand(IProcessQuery queryProcessor)
        {
            // name is required, has min/max lengths, and cannot already exist
            RuleFor(x => x.Name)
                .NotEmpty().WithName(User.Constraints.NameLabel)
                .MinLength(User.Constraints.NameMinLength)
                .MaxLength(User.Constraints.NameMaxLength)
                .MustNotFindUserByName(queryProcessor)
            ;
        }
    }

    public class HandleCreateUserCommand : IHandleCommand<CreateUser>
    {
        private readonly ICommandEntities _entities;

        public HandleCreateUserCommand(ICommandEntities entities)
        {
            _entities = entities;
        }

        public void Handle(CreateUser command)
        {
            var entity = new User { Name = command.Name };
            _entities.Create(entity);

            command.Created = entity;
        }
    }
}
