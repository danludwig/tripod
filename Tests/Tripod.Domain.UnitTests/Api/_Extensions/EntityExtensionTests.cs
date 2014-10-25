using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using Should;
using Tripod.Domain.Security;
using Xunit;

namespace Tripod
{
    public class EntityExtensionTests
    {
        [Fact]
        public void EagerLoad_ReturnsSameQueryable_WhenNotUsingEntitySet()
        {
            var queryable = new User[0].AsQueryable();
            var eagerLoaded = queryable.EagerLoad(new Expression<Func<User, object>>[]
                {
                    x => x.Permissions,
                })
            ;
            ReferenceEquals(queryable, eagerLoaded).ShouldBeTrue();
        }

        [Fact]
        public void EagerLoad_DelegatesToImplementation_WhenUsingEntitySet()
        {
            var data = new User[0].AsQueryable();
            var eagerLoad = new Expression<Func<User, object>>[]
            {
                x => x.Permissions,
            };
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var queryable = new EntitySet<User>(data, entities.Object);
            Expression<Func<Expression<Func<User, object>>, bool>> expectedExpression = x => eagerLoad.Contains(x);
            entities.Setup(x => x.EagerLoad(data, It.Is(expectedExpression))).Returns(data);
            var eagerLoaded = queryable.EagerLoad(eagerLoad);

            ReferenceEquals(queryable, eagerLoaded).ShouldBeTrue();
            entities.Verify(x => x.EagerLoad(data, It.Is(expectedExpression)), Times.Once);
        }

        [Fact]
        public void OrderBy_DoesNotSort_WhenExpressionsIsNull()
        {
            var data = new[]
            {
                new User { Name = "user2" },
                new User { Name = "user3" },
                new User { Name = "user1" },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(null);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Last().ShouldEqual(data.Last());
        }

        [Fact]
        public void OrderBy_SortsSimpleQueryable_Ascending()
        {
            var data = new[]
            {
                new User { Name = "user2" },
                new User { Name = "user3" },
                new User { Name = "user1" },
            };
            var orderBy = new Dictionary<Expression<Func<User, object>>, OrderByDirection>
            {
                { x => x.Name, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Last());
            orderedQueryable.Skip(1).First().ShouldEqual(data.First());
            orderedQueryable.Last().ShouldEqual(data.Skip(1).First());
        }

        [Fact]
        public void OrderBy_SortsSimpleQueryable_Descending()
        {
            var data = new[]
            {
                new User { Name = "user1" },
                new User { Name = "user3" },
                new User { Name = "user2" },
            };
            var orderBy = new Dictionary<Expression<Func<User, object>>, OrderByDirection>
            {
                { x => x.Name, OrderByDirection.Descending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsCompoundQueryable_AscendingThenDescending()
        {
            var data = new[]
            {
                new FakeEntityWithCollectionProperty { Name = "user2", CollectionProperty = new FakeEntityWithSortableProperties[2] },
                new FakeEntityWithCollectionProperty { Name = "user2", CollectionProperty = new FakeEntityWithSortableProperties[1] },
                new FakeEntityWithCollectionProperty { Name = "user1", CollectionProperty = new FakeEntityWithSortableProperties[0] },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithCollectionProperty, object>>, OrderByDirection>
            {
                { x => x.Name, OrderByDirection.Ascending },
                { x => x.CollectionProperty.Count, OrderByDirection.Descending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Last());
            orderedQueryable.Skip(1).First().ShouldEqual(data.First());
            orderedQueryable.Last().ShouldEqual(data.Skip(1).First());
        }

        [Fact]
        public void OrderBy_SortsCompoundQueryable_DescendingThenAscending()
        {
            var data = new[]
            {
                new FakeEntityWithCollectionProperty { Name = "user1", CollectionProperty = new FakeEntityWithSortableProperties[0] },
                new FakeEntityWithCollectionProperty { Name = "user2", CollectionProperty = new FakeEntityWithSortableProperties[1] },
                new FakeEntityWithCollectionProperty { Name = "user2", CollectionProperty = new FakeEntityWithSortableProperties[2] },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithCollectionProperty, object>>, OrderByDirection>
            {
                { x => x.Name, OrderByDirection.Descending },
                { x => x.CollectionProperty.Count, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithDateTime()
        {
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { DateTime = DateTime.UtcNow.AddHours(2) } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { DateTime = DateTime.UtcNow } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { DateTime = DateTime.UtcNow.AddHours(1) } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.DateTime, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithNullableDateTime()
        {
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableDateTime = DateTime.UtcNow.AddHours(1) } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableDateTime = null } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableDateTime = DateTime.UtcNow } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.NullableDateTime, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithBool()
        {
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Bool = true } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Bool = false } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Bool = false } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.Bool, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithNullableBool()
        {
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableBool = true } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableBool = null } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableBool = false } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.NullableBool, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithInt()
        {
            var number = FakeData.Int(int.MinValue, int.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Int = number + 2 } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Int = number } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Int = number + 1 } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.Int, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithNullableInt()
        {
            var number = FakeData.Int(int.MinValue, int.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableInt = number + 1 } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableInt = null } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableInt = number } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.NullableInt, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithLong()
        {
            var number = FakeData.Int(int.MinValue, int.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Long = number + 2 } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Long = number } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Long = number + 1 } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.Long, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithNullableLong()
        {
            var number = FakeData.Int(int.MinValue, int.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableLong = number + 1 } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableLong = null } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableLong = number } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.NullableLong, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithShort()
        {
            var number = FakeData.Short(short.MinValue, short.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Short = (short)(number + 2) } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Short = number } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Short = (short)(number + 1) } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.Short, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithNullableShort()
        {
            var number = FakeData.Short(short.MinValue, short.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableShort = (short)(number + 1) } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableShort = null } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableShort = number } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.NullableShort, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithDouble()
        {
            double number = FakeData.Double(int.MinValue, int.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Double = number + 2 } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Double = number } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Double = number + 1 } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.Double, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithNullableDouble()
        {
            double number = FakeData.Double(int.MinValue, int.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableDouble = number + 1 } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableDouble = null } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableDouble = number } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.NullableDouble, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithFloat()
        {
            float number = FakeData.Float(short.MinValue, short.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Float = number + 2 } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Float = number } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Float = number + 1 } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.Float, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithNullableFloat()
        {
            float number = FakeData.Float(short.MinValue, short.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableFloat = number + 1 } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableFloat = null } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableFloat = number } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.NullableFloat, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithDecimal()
        {
            decimal number = FakeData.Decimal(int.MinValue, int.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Decimal = number + 2 } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Decimal = number } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Decimal = number + 1 } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.Decimal, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_SortsNavigationalQueryable_WithNullableDecimal()
        {
            decimal number = FakeData.Decimal(int.MinValue, int.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableDecimal = number + 1 } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableDecimal = null } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { NullableDecimal = number } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.NullableDecimal, OrderByDirection.Ascending },
            };
            var orderedQueryable = data.AsQueryable().OrderBy(orderBy);
            orderedQueryable.Count().ShouldEqual(3);
            orderedQueryable.First().ShouldEqual(data.Skip(1).First());
            orderedQueryable.Skip(1).First().ShouldEqual(data.Last());
            orderedQueryable.Last().ShouldEqual(data.First());
        }

        [Fact]
        public void OrderBy_ThrowsNotSupportedException_WithByte()
        {
            byte number = FakeData.Byte(byte.MinValue, byte.MaxValue - 3);
            var data = new[]
            {
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Byte = (byte)(number + 2) } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Byte = number } },
                new FakeEntityWithNavigationProperty { NavigationProperty = new FakeEntityWithSortableProperties { Byte = (byte)(number + 1) } },
            };
            var orderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
            {
                { x => x.NavigationProperty.Byte, OrderByDirection.Ascending },
            };
            var exception = Assert.Throws<NotSupportedException>(() => data.AsQueryable().OrderBy(orderBy));
            exception.ShouldNotBeNull();
            exception.Message.ShouldEqual("OrderBy object type resolution is not yet implemented for 'Byte'.");
        }

        [Fact]
        public void ByIntId_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new Permission { Name = FakeData.String() },
                new Permission { Name = FakeData.String() },
                new Permission { Name = FakeData.String() },
            };
            data.AsQueryable().ById(int.MaxValue).ShouldBeNull();
        }

        [Fact]
        public void ByIntId_Queryable_CanDisallowNull()
        {
            var data = new Permission[]
            {
                new FakeEntityWithId1(),
                new FakeEntityWithId2(),
                new FakeEntityWithId3(),
            };
            data.AsQueryable().ById(2, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsQueryable().ById(int.MaxValue, false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByIntId_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new Permission { Name = FakeData.String() },
                new Permission { Name = FakeData.String() },
                new Permission { Name = FakeData.String() },
            };
            data.AsEnumerable().ById(int.MaxValue).ShouldBeNull();
        }

        [Fact]
        public void ByIntId_Enumerable_CanDisallowNull()
        {
            var data = new Permission[]
            {
                new FakeEntityWithId1(),
                new FakeEntityWithId2(),
                new FakeEntityWithId3(),
            };
            data.AsEnumerable().ById(3, false).ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                data.AsEnumerable().ById(int.MaxValue, false));
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByIntIdAsync_Queryable_CanAllowNull()
        {
            var data = new[]
            {
                new Permission { Name = FakeData.String() },
                new Permission { Name = FakeData.String() },
                new Permission { Name = FakeData.String() },
            };
            var dbSet = new Mock<DbSet<Permission>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByIdAsync(int.MaxValue).Result.ShouldBeNull();
        }

        [Fact]
        public void ByIntIdAsync_Queryable_CanDisallowNull()
        {
            var data = new Permission[]
            {
                new FakeEntityWithId1(),
                new FakeEntityWithId2(),
                new FakeEntityWithId3(),
            };
            var dbSet = new Mock<DbSet<Permission>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsQueryable().ByIdAsync(1, false).Result.ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsQueryable().ByIdAsync(int.MaxValue, false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }

        [Fact]
        public void ByIntIdAsync_Enumerable_CanAllowNull()
        {
            var data = new[]
            {
                new Permission { Name = FakeData.String() },
                new Permission { Name = FakeData.String() },
                new Permission { Name = FakeData.String() },
            };
            var dbSet = new Mock<DbSet<Permission>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByIdAsync(int.MaxValue).Result.ShouldBeNull();
        }

        [Fact]
        public void ByIntIdAsync_Enumerable_CanDisallowNull()
        {
            var data = new[]
            {
                new Permission { Name = FakeData.String() },
                new FakeEntityWithId1(),
                new FakeEntityWithId2(),
                new FakeEntityWithId3(),
            };
            var dbSet = new Mock<DbSet<Permission>>(MockBehavior.Strict).SetupDataAsync(data.AsQueryable());
            dbSet.Object.AsEnumerable().ByIdAsync(0, false).Result.ShouldNotBeNull();
            var exception = Assert.Throws<InvalidOperationException>(() =>
                dbSet.Object.AsEnumerable().ByIdAsync(int.MaxValue, false).Result);
            Assert.NotNull(exception);
            exception.Message.IndexOf("Sequence contains no matching element", StringComparison.CurrentCulture)
                .ShouldEqual(0);
        }
    }
}
