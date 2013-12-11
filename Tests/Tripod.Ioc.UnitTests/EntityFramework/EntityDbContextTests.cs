using System;
using System.Data.Entity;
using Edm_EntityMappingGeneratedViews;
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
        public void Views_GetView_ThrowsArgumentNullException_WhenExtentIsNull()
        {
            var views = new ViewsForBaseEntitySets7dcbd53ce5882e0e43365c556945dbafb63b07561d8262b094390e04057869e9();
            var exception = Assert.Throws<ArgumentNullException>(() => views.GetView(null));
            Assert.NotNull(exception);
            exception.ParamName.ShouldEqual("extent");
        }
    }
}
