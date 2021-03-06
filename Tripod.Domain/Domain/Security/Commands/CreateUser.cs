﻿using System;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class CreateUser : BaseCreateEntityCommand<User>, IDefineCommand
    {
        internal CreateUser() { }
        internal string Name { get; set; }
    }

    public class ValidateCreateUserCommand : AbstractValidator<CreateUser>
    {
        public ValidateCreateUserCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Name)
                .MustBeValidUserName()
                .MustNotFindUserByName(queries)
                    .WithName(User.Constraints.NameLabel)
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

            command.CreatedEntity = entity;
            return Task.FromResult(0);
        }
    }
}
