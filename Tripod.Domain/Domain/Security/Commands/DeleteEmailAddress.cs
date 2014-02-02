using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class DeleteEmailAddress : IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
        public int EmailAddressId { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class ValidateDeleteEmailAddressCommand : AbstractValidator<DeleteEmailAddress>
    {
        public ValidateDeleteEmailAddressCommand(IProcessQueries queries)
        {
            RuleFor(x => x.Principal)
                // must find user by principal
                .MustFindUserByPrincipal(queries)
                    .WithName(User.Constraints.Label)
            ;

            RuleFor(x => x.EmailAddressId)
                // email address must exist
                .MustFindEmailAddressById(queries)

                // user/principal must own the email address
                .MustBeEmailAddressWithOwnerId(queries, x => x.Principal.Identity.GetAppUserId())

                // cannot delete primary email address
                // also, cannot delete only email address
                // this rule protects both, since last email should always be the primary
                .MustNotBePrimaryEmailAddress(queries)

                    // only need to validate this field when there is a principal
                    .When(x => x.Principal != null)
                    .WithName(EmailAddress.Constraints.Label)
            ;
        }
    }

    [UsedImplicitly]
    public class HandleDeleteEmailAddressCommand : IHandleCommand<DeleteEmailAddress>
    {
        private readonly IWriteEntities _entities;

        public HandleDeleteEmailAddressCommand(IWriteEntities entities)
        {
            _entities = entities;
        }

        public async Task Handle(DeleteEmailAddress command)
        {
            // load up the entity
            var entity = await _entities.GetAsync<EmailAddress>(command.EmailAddressId);

            // delete if it is verified
            if (entity.IsVerified)
            {
                _entities.Delete(entity);
            }

            // when it is unverified, just detach from owner
            else
            {
                entity.OwnerId = null;
            }

            await _entities.SaveChangesAsync();
        }
    }
}
