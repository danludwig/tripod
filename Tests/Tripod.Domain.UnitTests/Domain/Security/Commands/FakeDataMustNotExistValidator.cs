using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeDataMustNotExistValidator : AbstractValidator<FakeDataCommand>
    {
        public FakeDataMustNotExistValidator(IProcessQueries queries)
        {
            RuleFor(x => x.UserName).MustNotFindUserByName(queries).WithName(User.Constraints.NameLabel);
        }
    }
}