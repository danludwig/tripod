using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Authenticate user's local membership.
    /// </summary>
    public class VerifyConfirmEmailSecret : IDefineCommand
    {
        public string Ticket { get; set; }
        public string Secret { get; set; }
    }

    public class ValidateVerifyConfirmEmailSecretCommand : AbstractValidator<VerifyConfirmEmailSecret>
    {
        public ValidateVerifyConfirmEmailSecretCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Secret)
                .NotEmpty()
                    .WithName(EmailConfirmation.Constraints.SecretLabel);
        }
    }

    public class HandleVerifyConfirmEmailSecretCommand : IHandleCommand<VerifyConfirmEmailSecret>
    {
        //private readonly UserManager<User, int> _userManager;
        //private readonly IProcessQueries _queries;
        private readonly IReadEntities _entities;
        //private readonly IDeliverEmailMessage _mail;

        public HandleVerifyConfirmEmailSecretCommand(IWriteEntities entities)
        {
            //_userManager = userManager;
            //_queries = queries;
            _entities = entities;
            //_mail = mail;
        }

        public async Task Handle(VerifyConfirmEmailSecret command)
        {
        }
    }
}
