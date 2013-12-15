using System;
using System.Linq.Expressions;
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
            Expression<Func<FakeCommandWithValidator, bool>> expectedCommand = y => ReferenceEquals(y, command);
            decorated.Setup(x => x.Handle(It.Is(expectedCommand)));
            var decorator = new CommandLifetimeScopeDecorator<FakeCommandWithValidator>(Container, () => decorated.Object);
            Container.GetCurrentLifetimeScope().ShouldEqual(null);
            decorator.Handle(command);
            Container.GetCurrentLifetimeScope().ShouldEqual(null);
            decorated.Verify(x => x.Handle(It.Is(expectedCommand)), Times.Once);
        }

        [Fact]
        public void UsesCurrentLifetimeScope_WhenCurrentLifetimeScope_IsNotNull()
        {
            var command = new FakeCommandWithValidator();
            var decorated = new Mock<IHandleCommand<FakeCommandWithValidator>>(MockBehavior.Strict);
            Expression<Func<FakeCommandWithValidator, bool>> expectedCommand = y => ReferenceEquals(y, command);
            decorated.Setup(x => x.Handle(It.Is(expectedCommand)));
            var decorator = new CommandLifetimeScopeDecorator<FakeCommandWithValidator>(Container, () => decorated.Object);
            Container.GetCurrentLifetimeScope().ShouldEqual(null);
            using (Container.BeginLifetimeScope())
            {
                decorator.Handle(command);
            }
            Container.GetCurrentLifetimeScope().ShouldEqual(null);
            decorated.Verify(x => x.Handle(It.Is(expectedCommand)), Times.Once);
        }
    }
}
