using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeMustFindEmailAddressByIdCommand
    {
        public int EmailAddressId { get; set; }
    }

    public class FakeMustFindEmailAddressByIdValidator : AbstractValidator<FakeMustFindEmailAddressByIdCommand>
    {
        public FakeMustFindEmailAddressByIdValidator(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddressId)
                .MustFindEmailAddressById(queries)
                .WithName(EmailAddress.Constraints.Label)
            ;
        }
    }
}