namespace Tripod.Ioc.Transactions
{
    public class FakeCommandWithoutValidator : IDefineCommand
    {
        public string ReturnValue { [UsedImplicitly] get; internal set; }
    }

    [UsedImplicitly]
    public class HandleFakeCommandWithoutValidator : IHandleCommand<FakeCommandWithoutValidator>
    {
        public void Handle(FakeCommandWithoutValidator command)
        {
            command.ReturnValue = "faked";
        }
    }
}