using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Linq;
using System.Reflection;
using Moq;
using Should;
using Xunit;

namespace Tripod.Ioc.EntityFramework
{
    public class EntityDbContextTests
    {
        [Fact]
        public void Ctor_SetsInitializer_ToBrownfield()
        {
            var dbContext = new EntityDbContext();
            dbContext.Initializer.ShouldNotBeNull();
            dbContext.Initializer.GetType().ShouldEqual(typeof(BrownfieldDbInitializer));
        }

        [Fact]
        public void Initializer_HasPublicSetter()
        {
            var initializer = new Mock<IDatabaseInitializer<EntityDbContext>>(MockBehavior.Loose);
            var dbContext = new EntityDbContext
            {
                Initializer = initializer.Object
            };
            dbContext.Initializer.ShouldEqual(initializer.Object);
        }

        [Fact]
        public void ModelCreator_HasPublicSetter()
        {
            var modelCreator = new Mock<ICreateDbModel>(MockBehavior.Loose);
            var dbContext = new EntityDbContext
            {
                ModelCreator = modelCreator.Object
            };
            dbContext.ModelCreator.ShouldEqual(modelCreator.Object);
        }

        [Fact]
        public void DbMappingViewCache_ThrowsArgumentNullException_WhenExtentIsNull()
        {
            var assembly = Assembly.GetAssembly(typeof(EntityDbContext));
            var typesToTest = assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(DbMappingViewCache).IsAssignableFrom(t))
                .ToArray();
            if (!typesToTest.Any()) return;
            foreach (var instance in typesToTest.Select(typeToTest => Activator.CreateInstance(typeToTest) as DbMappingViewCache))
            {
                Assert.NotNull(instance);
                var exception = Assert.Throws<ArgumentNullException>(() => instance.GetView(null));
                exception.ShouldNotBeNull();
                exception.ParamName.ShouldEqual("extent");
            }
        }
    }
}
