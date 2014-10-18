using System.Reflection;
using Should;
using SimpleInjector;
using SimpleInjector.Extensions;
using Xunit;

namespace Tripod.Services.Transactions
{
    public class CommandProcessorTests
    {
        [Fact]
        public void Execute_InvokesCommandHandler_UsingContainerForResolution()
        {
            var container = new Container();
            container.RegisterSingle<IProcessCommands, CommandProcessor>();
            container.RegisterManyForOpenGeneric(typeof(IHandleCommand<>), Assembly.GetExecutingAssembly());
            container.Verify();
            var commands = container.GetInstance<IProcessCommands>();
            var command = new FakeCommandWithoutValidator();

            commands.Execute(command);

            command.ReturnValue.ShouldEqual("faked");
        }
    }
}
