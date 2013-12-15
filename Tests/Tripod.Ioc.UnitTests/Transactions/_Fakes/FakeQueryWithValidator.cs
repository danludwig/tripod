using FluentValidation;

namespace Tripod.Ioc.Transactions
{
    public class FakeQueryWithValidator : IDefineQuery<string>
    {
        public string InputValue { get; set; }
    }

    [UsedImplicitly]
    public class ValidateFakeQuery : AbstractValidator<FakeQueryWithValidator>
    {
        public ValidateFakeQuery()
        {
            RuleFor(x => x.InputValue).NotEmpty();
        }
    }

    [UsedImplicitly]
    public class HandleFakeQueryWithValidator : IHandleQuery<FakeQueryWithValidator, string>
    {
        public string Handle(FakeQueryWithValidator query)
        {
            return "faked";
        }
    }
}