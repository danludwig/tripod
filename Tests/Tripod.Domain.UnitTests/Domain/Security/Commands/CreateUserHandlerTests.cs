using System;
using System.Linq.Expressions;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class CreateUserHandlerTests
    {
        [Fact]
        public void CreatesUserEntity()
        {
            var command = new CreateUser { Name = "new" };
            var entities = new Mock<ICommandEntities>(MockBehavior.Strict);
            var handler = new HandleCreateUserCommand(entities.Object);
            Expression<Func<User, bool>> expectedEntity = y => y.Name.Equals(command.Name);
            entities.Setup(x => x.Create(It.Is(expectedEntity)));

            handler.Handle(command);

            entities.Verify(x => x.Create(It.Is(expectedEntity)), Times.Once);
        }

        [Fact]
        public void SetsCreatedProperty_OnCommand()
        {
            var command = new CreateUser { Name = "new" };
            var entities = new Mock<ICommandEntities>(MockBehavior.Loose);
            var handler = new HandleCreateUserCommand(entities.Object);

            handler.Handle(command);

            command.Created.ShouldNotBeNull();
            command.Created.Name.ShouldEqual(command.Name);
        }
    }
}
