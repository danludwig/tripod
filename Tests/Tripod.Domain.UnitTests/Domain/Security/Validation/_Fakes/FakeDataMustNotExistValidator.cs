using System.Diagnostics;
using FluentValidation;

namespace Tripod.Domain.Security
{
    public class FakeDataMustNotExistValidator : AbstractValidator<FakeDataCommand>
    {
        public FakeDataMustNotExistValidator(IProcessQueries queries)
        {
            When(x => !x.UserId.HasValue, () =>
                RuleFor(x => x.UserName)
                    .MustNotFindUserByName(queries)
                    .WithName(User.Constraints.NameLabel)
            );

            When(x => x.UserId.HasValue, () =>
                RuleFor(x => x.UserName)
                    .MustNotFindUserByName(queries, x =>
                    {
                        Debug.Assert(x.UserId.HasValue);
                        return x.UserId.Value;
                    })
                .WithName(User.Constraints.NameLabel));
        }
    }
}