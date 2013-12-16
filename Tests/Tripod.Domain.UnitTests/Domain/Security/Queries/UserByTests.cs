using System;
using System.Data.Entity;
using System.Linq;
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
            query.Id.HasValue.ShouldBeFalse();
        }

        [Fact]
        public void Handler_ThrowsArgumentNullException_WhenQueryIsNull()
        {
            var handler = new HandleUserByQuery(null);
            var exception = Assert.Throws<ArgumentNullException>(() => handler.Handle(null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("query");
        }

        [Fact]
        public void Handler_InvokesQueryUser_Once_OnIQueryEntities()
        {
            var userName = Guid.NewGuid().ToString();
            var data = new[] { new User { Name = userName } }.AsQueryable();
            var query = new UserBy(userName);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IQueryEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            var handler = new HandleUserByQuery(entities.Object);

            var result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.ShouldEqual(data.Single());
            entities.Verify(x => x.Query<User>(), Times.Once);
        }
    }
}
