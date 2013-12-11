using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Should;
using Tripod.Domain.Security;
using Xunit;

namespace Tripod.Ioc.EntityFramework
{
    public class EntityDbContextTests : EntityDbContextIntegrationTests
    {
        [Fact]
        public void DatabaseName_IsTestName()
        {
            using (var dbContext = new EntityDbContext())
            {
                dbContext.Database.Connection.Database.ShouldEqual("TripodIocIntegrationTestDb");
            }
        }

        [Fact]
        public void SingleOrDefaultAsync_ReturnsNull_WhenQueryableIsNull()
        {
            using (var dbContext = new EntityDbContext())
            {
                var result = dbContext.SingleOrDefaultAsync<Permission>(null, x => x != null).Result;
                result.ShouldBeNull();
            }
        }

        [Fact]
        public void EagerLoad_IncludesRelatedData()
        {
            using (var dbContext = new EntityDbContext())
            {
                var userName = Guid.NewGuid().ToString();
                var permissionName = Guid.NewGuid().ToString();
                var user = new User
                {
                    Name = userName,
                    Permissions = new[] { new Permission(permissionName) },
                };
                dbContext.Create(user);
                var affectedRows = dbContext.SaveChangesAsync().Result;
                affectedRows.ShouldEqual(3);

                var entity = dbContext.Query<User>()
                    .EagerLoad(new Expression<Func<User, object>>[]
                {
                    x => x.Permissions,
                }).Single(x => x.Name.Equals(userName));
                entity.Permissions.Count.ShouldEqual(1);
                entity.Permissions.Single().Name.ShouldEqual(permissionName);
            }
        }

        [Fact]
        public void EagerLoad_ReturnsNull_WhenQueryIsNull()
        {
            using (var dbContext = new EntityDbContext())
            {
                var result = dbContext.EagerLoad<Permission>(null, x => x.Users);
                result.ShouldBeNull();
            }
        }

        [Fact]
        public void Query_ReturnsData()
        {
            using (var dbContext = new EntityDbContext())
            {
                var createdEntity = new User { Name = Guid.NewGuid().ToString() };
                dbContext.Create(createdEntity);
                var affectedRows = dbContext.SaveChangesAsync().Result;
                affectedRows.ShouldEqual(1);

                var queriedEntity = dbContext.Query<User>().SingleOrDefaultAsync(x => x.Id == createdEntity.Id).Result;

                Assert.NotNull(queriedEntity);
                createdEntity.Id.ShouldEqual(queriedEntity.Id);
            }
        }

        [Fact]
        public void NoArgGet_ReturnsDataFromStore()
        {
            using (var dbContext = new EntityDbContext())
            {
                var entities = dbContext.Get<User>().Take(2).ToArray();

                entities.ShouldNotBeNull();
                entities.Length.ShouldBeInRange(0, 2);
            }
        }

        [Fact]
        public void Get_ThrowsArgumentNullException_WhenFirstKeyValueArgumentIsNull()
        {
            using (var dbContext = new EntityDbContext())
            {
                var exception = Assert.Throws<ArgumentNullException>(() => dbContext.Get<User>(null));
                exception.ShouldNotBeNull();
                exception.ParamName.ShouldEqual("firstKeyValue");
            }
        }

        [Fact]
        public void Get_ReturnsNull_WhenPrimaryKeyDoesNotMatchRow()
        {
            using (var dbContext = new EntityDbContext())
            {
                var entity = dbContext.Get<User>(int.MaxValue);
                entity.ShouldBeNull();
            }
        }

        [Fact]
        public void Create_SetsEntityState_ToAdded()
        {
            using (var dbContext = new EntityDbContext())
            {
                var entity = new User { Name = Guid.NewGuid().ToString() };
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Detached);
                dbContext.Create(entity);
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Added);
            }
        }

        [Fact]
        public void Create_AddsEntityToDb_WhenChangesAreSaved()
        {
            using (var dbContext = new EntityDbContext())
            {
                var entity = new User { Name = Guid.NewGuid().ToString() };
                dbContext.Create(entity);
                entity.Id.ShouldEqual(0);
                var affectedRows = dbContext.SaveChangesAsync().Result;
                affectedRows.ShouldEqual(1);
                entity.Id.ShouldNotEqual(0);
            }
        }

        [Fact]
        public void Update_SetsEntityState_ToModified()
        {
            using (var dbContext = new EntityDbContext())
            {
                var permissionName = Guid.NewGuid().ToString();
                var entity = new Permission(permissionName) { Description = "d1" };
                dbContext.Create(entity);
                var affectedRows = dbContext.SaveChangesAsync().Result;

                affectedRows.ShouldEqual(1);
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                dbContext.Update(entity);
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Modified);
            }
        }

        [Fact]
        public void Delete_SetsEntityState_ToDeleted()
        {
            using (var dbContext = new EntityDbContext())
            {
                var entity = new User
                {
                    Name = Guid.NewGuid().ToString()
                };
                dbContext.Create(entity);
                var affectedRows = dbContext.SaveChangesAsync().Result;

                affectedRows.ShouldEqual(1);
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                dbContext.Delete(entity);
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Deleted);
            }
        }

        [Fact]
        public void ReloadAsync_ChangesModifiedEntityState_ToUnchanged()
        {
            using (var dbContext = new EntityDbContext())
            {
                var description = Guid.NewGuid().ToString();
                var entity = new Permission(Guid.NewGuid().ToString()) { Description = description };
                dbContext.Create(entity);
                var affectedRows = dbContext.SaveChangesAsync().Result;

                affectedRows.ShouldEqual(1);
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                entity.Description = Guid.NewGuid().ToString();
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Modified);
                dbContext.ReloadAsync(entity).Wait();
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                entity.Description.ShouldEqual(description);
            }
        }

        [Fact]
        public void DiscardChanges_ChangesAddedEntityState_ToDetached()
        {
            using (var dbContext = new EntityDbContext())
            {
                var entity = new User { Name = Guid.NewGuid().ToString() };
                dbContext.Create(entity);

                dbContext.Entry(entity).State.ShouldEqual(EntityState.Added);
                dbContext.DiscardChangesAsync().Wait();
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Detached);
            }
        }

        [Fact]
        public void DiscardChanges_ChangesModifiedEntityState_ToUnchanged()
        {
            using (var dbContext = new EntityDbContext())
            {
                var userName = Guid.NewGuid().ToString();
                var entity = new User { Name = userName };
                dbContext.Create(entity);
                var affectedRows = dbContext.SaveChangesAsync().Result;

                affectedRows.ShouldEqual(1);
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                entity.Name = Guid.NewGuid().ToString();
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Modified);
                dbContext.DiscardChangesAsync().Wait();
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                entity.Name.ShouldEqual(userName);

            }
        }

        [Fact]
        public void DiscardChanges_ChangesDeletedEntityState_ToUnchanged()
        {
            using (var dbContext = new EntityDbContext())
            {
                var entity = new User { Name = Guid.NewGuid().ToString() };
                dbContext.Create(entity);
                var affectedRows = dbContext.SaveChangesAsync().Result;

                affectedRows.ShouldEqual(1);
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                dbContext.Delete(entity);
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Deleted);
                dbContext.DiscardChangesAsync().Wait();
                dbContext.Entry(entity).State.ShouldEqual(EntityState.Unchanged);

            }
        }
    }
}
