using System.Threading.Tasks;
using Moq;
using Should;
using SimpleInjector;
using Xunit;

namespace Tripod.Ioc.Transactions
{
    public class CommandLifetimeScopeDecoratorTests : SimpleInjectorContainerTests
    {
        [Fact]
        public void BeginsLifetimeScope_WhenCurrentLifetimeScope_IsNull()
        {
            var command = new FakeCommandWithValidator();
            var decorated = new Mock<IHandleCommand<FakeCommandWithValidator>>(MockBehavior.Strict);
            decorated.Setup(x => x.Handle(command)).Returns(Task.FromResult(0));
            var decorator = new CommandLifetimeScopeDecorator<FakeCommandWithValidator>(Container, () => decorated.Object);
            Container.GetCurrentLifetimeScope().ShouldEqual(null);
            decorator.Handle(command);
            Container.GetCurrentLifetimeScope().ShouldEqual(null);
            decorated.Verify(x => x.Handle(command), Times.Once);
        }

        [Fact]
        public void UsesCurrentLifetimeScope_WhenCurrentLifetimeScope_IsNotNull()
        {
            var command = new FakeCommandWithValidator();
            var decorated = new Mock<IHandleCommand<FakeCommandWithValidator>>(MockBehavior.Strict);
            decorated.Setup(x => x.Handle(command)).Returns(Task.FromResult(0));
            var decorator = new CommandLifetimeScopeDecorator<FakeCommandWithValidator>(Container, () => decorated.Object);
            Container.GetCurrentLifetimeScope().ShouldEqual(null);
            using (Container.BeginLifetimeScope())
            {
                decorator.Handle(command);
            }
            Container.GetCurrentLifetimeScope().ShouldEqual(null);
            decorated.Verify(x => x.Handle(command), Times.Once);
        }
    }
}
