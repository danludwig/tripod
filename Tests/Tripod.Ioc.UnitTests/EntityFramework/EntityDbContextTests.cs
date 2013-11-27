using System.Data.Entity;
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
            dbContext.Initializer.GetType().ShouldEqual(typeof(BrownfieldDbInitializer<EntityDbContext>));
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
    }
}
