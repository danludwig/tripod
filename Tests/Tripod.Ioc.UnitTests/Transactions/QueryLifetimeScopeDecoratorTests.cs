using System;
using System.Linq.Expressions;
using Moq;
using Should;
using SimpleInjector;
using Xunit;

namespace Tripod.Ioc.Transactions
{
    public class QueryLifetimeScopeDecoratorTests : SimpleInjectorContainerTests
    {
        [Fact]
        public void BeginsLifetimeScope_WhenCurrentLifetimeScope_IsNull()
        {
            var query = new FakeQueryWithoutValidator();
            var decorated = new Mock<IHandleQuery<FakeQueryWithoutValidator, string>>(MockBehavior.Strict);
            Expression<Func<FakeQueryWithoutValidator, bool>> expectedQuery = x => ReferenceEquals(x, query);
            decorated.Setup(x => x.Handle(It.Is(expectedQuery))).Returns("faked");
            var decorator = new QueryLifetimeScopeDecorator<FakeQueryWithoutValidator, string>(Container, () => decorated.Object);
            Container.GetCurrentLifetimeScope().ShouldEqual(null);

            var result = decorator.Handle(query);

            result.ShouldEqual("faked");
            decorated.Verify(x => x.Handle(It.Is(expectedQuery)), Times.Once);
        }

        [Fact]
        public void UsesCurrentLifetimeScope_WhenCurrentLifetimeScope_IsNotNull()
        {
            var query = new FakeQueryWithoutValidator();
            var decorated = new Mock<IHandleQuery<FakeQueryWithoutValidator, string>>(MockBehavior.Strict);
            Expression<Func<FakeQueryWithoutValidator, bool>> expectedQuery = x => ReferenceEquals(x, query);
            decorated.Setup(x => x.Handle(It.Is(expectedQuery))).Returns("faked");
            var decorator = new QueryLifetimeScopeDecorator<FakeQueryWithoutValidator, string>(Container, () => decorated.Object);
            Container.GetCurrentLifetimeScope().ShouldEqual(null);
            string result;
            using (Container.BeginLifetimeScope())
            {
                result = decorator.Handle(query);
            }
            result.ShouldEqual("faked");
            decorated.Verify(x => x.Handle(It.Is(expectedQuery)), Times.Once);
        }
    }
}
