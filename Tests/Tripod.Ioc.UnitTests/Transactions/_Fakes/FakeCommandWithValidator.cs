using FluentValidation;

namespace Tripod.Ioc.Transactions
{
    public class FakeCommandWithValidator : IDefineCommand
    {
        public string InputValue { get; set; }
        public string ReturnValue { get; internal set; }
    }

    [UsedImplicitly]
    public class ValidateFakeCommand : AbstractValidator<FakeCommandWithValidator>
    {
        public ValidateFakeCommand()
        {
            RuleFor(x => x.InputValue).NotEmpty();
        }
    }

    public class HandleFakeCommandWithValidator : IHandleCommand<FakeCommandWithValidator>
    {
        public void Handle(FakeCommandWithValidator command)
        {
            command.ReturnValue = "faked";
        }
    }
}