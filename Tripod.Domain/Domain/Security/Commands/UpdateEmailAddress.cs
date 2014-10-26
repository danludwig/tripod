using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class UpdateEmailAddress : IDefineSecuredCommand
    {
        public IPrincipal Principal { get; set; }
        public int EmailAddressId { get; [UsedImplicitly] set; }
        public bool? IsPrimary { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public class ValidateUpdateEmailAddressCommand : AbstractValidator<UpdateEmailAddress>
    {
        public ValidateUpdateEmailAddressCommand(IProcessQueries queries)
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
                .MustBeEmailAddressWithUserId(queries, x => x.Principal.Identity.GetUserId<int>())

                // cannot set primary email address to false, only to true
                .MustNotBePrimaryEmailAddress(queries)
                    .When(x => x.IsPrimary == false, ApplyConditionTo.CurrentValidator)

                    // only need to validate this field when there is a principal
                .When(x => x.Principal != null)
                .WithName(EmailAddress.Constraints.Label)
            ;
        }
    }

    [UsedImplicitly]
    public class HandleUpdateEmailAddressCommand : IHandleCommand<UpdateEmailAddress>
    {
        private readonly IWriteEntities _entities;

        public HandleUpdateEmailAddressCommand(IWriteEntities entities)
        {
            _entities = entities;
        }

        public async Task Handle(UpdateEmailAddress command)
        {
            // load up the entity
            var entity = await _entities.Get<EmailAddress>()
                .EagerLoad(x => x.User.EmailAddresses)
                .ByIdAsync(command.EmailAddressId);
            var commit = false;

            // update the isprimary field when requested to
            if (command.IsPrimary.HasValue && entity.IsPrimary != command.IsPrimary.Value)
            {
                if (entity.IsPrimary && !command.IsPrimary.Value)
                    // todo: unit test validation to make sure this exception can never be thrown.
                    throw new InvalidOperationException(string.Format(
                        "Cannot set EmailAddress.IsPrimary to false for #{0}. This should have been caught in the validator.", entity.Id));

                // since the primary email address cannot be changed to non-primary,
                // at this point we must be changing a non-primary to primary.
                commit = true;
                var primaryEmail = entity.User.EmailAddresses.Single(x => x.IsPrimary);
                primaryEmail.IsPrimary = false;
                entity.IsPrimary = true;
            }

            if (commit) await _entities.SaveChangesAsync();
        }
    }
}
