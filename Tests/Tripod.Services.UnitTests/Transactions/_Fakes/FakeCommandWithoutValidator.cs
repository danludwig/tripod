using System.Threading.Tasks;

namespace Tripod.Services.Transactions
{
    public class FakeCommandWithoutValidator : IDefineCommand
    {
        public string ReturnValue { [UsedImplicitly] get; internal set; }
    }

    [UsedImplicitly]
    public class HandleFakeCommandWithoutValidator : IHandleCommand<FakeCommandWithoutValidator>
    {
        public Task Handle(FakeCommandWithoutValidator command)
        {
            command.ReturnValue = "faked";
            return Task.FromResult(0);
        }
    }
}