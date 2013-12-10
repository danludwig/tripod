using System;
using System.Data.Entity;
using System.Linq;
using Should;
using Tripod.Domain.Security;
using Xunit;

namespace Tripod.Ioc.EntityFramework
{
    public class EntityDbContextTests : DbContextIntegrationTest
    {
        [Fact]
        public void DatabaseName_IsTestName()
        {
            var dbContext = new EntityDbContext();
            dbContext.Database.Connection.Database.ShouldEqual("TripodIocIntegrationTestDb");
        }

        [Fact]
        public void NoArgGet_ReturnsDataFromStore()
        {
            var dbContext = new EntityDbContext();
            var dbInitializer = new CreateDatabaseIfNotExists<EntityDbContext>();
            dbContext.Initializer = dbInitializer;

            var entities = dbContext.Get<User>().Take(2).ToArray();

            entities.ShouldNotBeNull();
            entities.Length.ShouldBeInRange(0, 2);
        }

        [Fact]
        public void Query_ReturnsData()
        {
            var dbContext = new EntityDbContext();
            var dbInitializer = new CreateDatabaseIfNotExists<EntityDbContext>();
            dbContext.Initializer = dbInitializer;
            var createdEntity = new User { Name = Guid.NewGuid().ToString() };
            dbContext.Create(createdEntity);
            dbContext.SaveChanges();

            var queriedEntity = dbContext.Query<User>().SingleOrDefaultAsync(x => x.Id == createdEntity.Id).Result;

            Assert.NotNull(queriedEntity);
            createdEntity.Id.ShouldEqual(queriedEntity.Id);
        }

        [Fact]
        public void Get_ReturnsNull_WhenPrimaryKeyDoesNotMatchRow()
        {
            var dbContext = new EntityDbContext();
            var dbInitializer = new CreateDatabaseIfNotExists<EntityDbContext>();
            dbContext.Initializer = dbInitializer;

            var entity = dbContext.Get<User>(int.MaxValue);

            entity.ShouldBeNull();
        }

        [Fact]
        public void Create_SetsEntityState_ToAdded()
        {
            var dbContext = new EntityDbContext();
            var dbInitializer = new CreateDatabaseIfNotExists<EntityDbContext>();
            dbContext.Initializer = dbInitializer;

            var entity = new User { Name = Guid.NewGuid().ToString() };
            dbContext.Entry(entity).State.ShouldEqual(EntityState.Detached);
            dbContext.Create(entity);
            dbContext.Entry(entity).State.ShouldEqual(EntityState.Added);
        }

        [Fact]
        public void Create_AddsEntityToDb_WhenChangesAreSaved()
        {
            var dbContext = new EntityDbContext();
            var dbInitializer = new CreateDatabaseIfNotExists<EntityDbContext>();
            dbContext.Initializer = dbInitializer;
            var entity = new User { Name = Guid.NewGuid().ToString() };
            dbContext.Create(entity);
            entity.Id.ShouldEqual(0);
            dbContext.SaveChanges();
            entity.Id.ShouldNotEqual(0);

        }

        [Fact]
        public void Delete_SetsEntityState_ToDeleted()
        {
            var dbContext = new EntityDbContext();
            var dbInitializer = new CreateDatabaseIfNotExists<EntityDbContext>();
            dbContext.Initializer = dbInitializer;
            var entity = new User { Name = Guid.NewGuid().ToString() };
            dbContext.Create(entity);
            dbContext.SaveChanges();

            dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
            dbContext.Delete(entity);
            dbContext.Entry(entity).State.ShouldEqual(EntityState.Deleted);
        }

        [Fact]
        public void DiscardChanges_ChangesAddedEntityState_ToDetached()
        {
            var dbContext = new EntityDbContext();
            var dbInitializer = new CreateDatabaseIfNotExists<EntityDbContext>();
            dbContext.Initializer = dbInitializer;
            var entity = new User { Name = Guid.NewGuid().ToString() };
            dbContext.Create(entity);

            dbContext.Entry(entity).State.ShouldEqual(EntityState.Added);
            dbContext.DiscardChangesAsync().Wait();
            dbContext.Entry(entity).State.ShouldEqual(EntityState.Detached);
        }

        [Fact]
        public void DiscardChanges_ChangesDeletedEntityState_ToUnchanged()
        {
            var dbContext = new EntityDbContext();
            var dbInitializer = new CreateDatabaseIfNotExists<EntityDbContext>();
            dbContext.Initializer = dbInitializer;
            var entity = new User { Name = Guid.NewGuid().ToString() };
            dbContext.Create(entity);
            dbContext.SaveChanges();

            dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
            dbContext.Delete(entity);
            dbContext.Entry(entity).State.ShouldEqual(EntityState.Deleted);
            dbContext.DiscardChangesAsync().Wait();
            dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
        }
    }
}
