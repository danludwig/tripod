using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeMustBeEmailAddressWithUserIdCommand
    {
        public int UserId { get; set; }
        public int EmailAddressId { get; set; }
    }

    public class FakeMustBeEmailAddressWithUserIdValidator : AbstractValidator<FakeMustBeEmailAddressWithUserIdCommand>
    {
        public FakeMustBeEmailAddressWithUserIdValidator(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddressId)
                .MustBeEmailAddressWithUserId(queries, x => x.UserId)
                .WithName(EmailAddress.Constraints.Label)
            ;
        }
    }
}