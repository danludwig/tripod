using FluentValidation;

namespace Tripod.Ioc.Transactions
{
    public class FakeCommandWithValidator : IDefineCommand
    {
        public string ReturnValue { get; internal set; }
    }

    [UsedImplicitly]
    public class ValidateFakeCommand : AbstractValidator<FakeCommandWithValidator> { }

    public class HandleFakeCommandWithValidator : IHandleCommand<FakeCommandWithValidator>
    {
        public void Handle(FakeCommandWithValidator command)
        {
            command.ReturnValue = "faked";
        }
    }
}