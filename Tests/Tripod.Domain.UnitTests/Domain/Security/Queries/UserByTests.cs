using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class UserByTests
    {
        [Fact]
        public void Query_IntCtor_SetsIdProperty()
        {
            var id = new Random().Next(int.MinValue, int.MaxValue);
            var query = new UserBy(id);
            query.Id.ShouldEqual(id);
            query.Name.ShouldEqual(null);
        }

        [Fact]
        public void Query_StringCtor_SetsNameProperty()
        {
            var name = Guid.NewGuid().ToString();
            var query = new UserBy(name);
            query.Name.ShouldEqual(name);
            query.Id.HasValue.ShouldEqual(false);
        }

        [Fact]
        public void Handler_ThrowsArgumentNullException_WhenQueryIsNull()
        {
            var handler = new HandleUserByQuery(null);
            ArgumentNullException exception = null;
            try
            {
                handler.Handle(null).Wait();
            }
            catch (ArgumentNullException ex)
            {
                exception = ex;
            }
            catch (AggregateException ex)
            {
                ex.InnerExceptions.Count.ShouldEqual(1);
                exception = ex.InnerExceptions.Single() as ArgumentNullException;
            }
            Assert.NotNull(exception);
            exception.ParamName.ShouldEqual("query");
        }

        [Fact]
        public void Handler_InvokesQueryUser_Once_OnIQueryEntities()
        {
            var query = new UserBy(Guid.NewGuid().ToString());
            var entities = new Mock<IQueryEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(new User[0].AsQueryable(), entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            entities.Setup(x => x.SingleOrDefaultAsync(entitySet, It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult<User>(null));
            var handler = new HandleUserByQuery(entities.Object);

            var result = handler.Handle(query).Result;

            result.ShouldEqual(null);
            entities.Verify(x => x.Query<User>(), Times.Once());
        }

        [Fact]
        public void Handler_InvokesSingleOrDefaultAsync_Once_OnIQueryEntities()
        {
            var query = new UserBy(new Random().Next(int.MinValue, int.MaxValue));
            var entities = new Mock<IQueryEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(new User[0].AsQueryable(), entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            entities.Setup(x => x.SingleOrDefaultAsync(entitySet, It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult<User>(null));
            var handler = new HandleUserByQuery(entities.Object);

            var result = handler.Handle(query).Result;

            result.ShouldEqual(null);
            entities.Verify(x => x.SingleOrDefaultAsync(entitySet, It.IsAny<Expression<Func<User, bool>>>()), Times.Once());
        }
    }
}
