using System;
using System.Collections;
using System.Linq;
using Moq;
using Should;
using Tripod.Domain.Security;
using Xunit;

namespace Tripod
{
    public class EntitySetTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryableArgIsNull()
        {
            EntitySet<User> set = null;
            ArgumentNullException exception = null;
            try
            {
                set = new EntitySet<User>(null, null);
            }
            catch (ArgumentNullException ex)
            {
                exception = ex;
            }
            Assert.NotNull(exception);
            exception.ParamName.ShouldEqual("queryable");
            set.ShouldBeNull();
        }

        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenIReadEntitiesArgIsNull()
        {
            EntitySet<User> set = null;
            ArgumentNullException exception = null;
            try
            {
                set = new EntitySet<User>(new User[0].AsQueryable(), null);
            }
            catch (ArgumentNullException ex)
            {
                exception = ex;
            }
            Assert.NotNull(exception);
            exception.ParamName.ShouldEqual("entities");
            set.ShouldBeNull();
        }

        [Fact]
        public void GetEnumerator_IsImplemented_Generically()
        {
            var queryable = new User[0].AsQueryable();
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var set = new EntitySet<User>(queryable, entities.Object);

            var enumerator = set.GetEnumerator();

            Assert.NotNull(enumerator);
        }

        [Fact]
        public void GetEnumerator_IsImplemented_NonGenerically()
        {
            var queryable = new User[0].AsQueryable();
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var set = new EntitySet<User>(queryable, entities.Object) as IEnumerable;

            var enumerator = set.GetEnumerator();

            Assert.NotNull(enumerator);
        }

        [Fact]
        public void ElementType_DelegatesTo_Queryable()
        {
            var queryable = new User[0].AsQueryable();
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var set = new EntitySet<User>(queryable, entities.Object);

            var elementType = set.ElementType;

            Assert.NotNull(elementType);
            elementType.ShouldEqual(queryable.ElementType);
            elementType.ShouldEqual(typeof(User));
        }
    }
}
