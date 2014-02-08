using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Should;
using SimpleInjector;
using Xunit;

namespace Tripod.Services.EntityFramework
{
    public class CompositionRootTests : SimpleInjectorContainerTests
    {
        [Fact]
        public void RegistersICustomizeDb_UsingVanillaDbCustomizer_Transiently_WhenSettingIsNotGreenfield()
        {
            var instance = Container.GetInstance<ICustomizeDb>();
            var registration = Container.GetRegistration(typeof(ICustomizeDb));

            instance.ShouldNotBeNull();
            instance.ShouldBeType<VanillaDbCustomizer>();
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
        }

        [Fact]
        public void RegistersIDbInitializer_UsingBrownfieldDbInitializer_Transiently_WhenSettingIsNotGreenfield()
        {
            var instance = Container.GetInstance<IDatabaseInitializer<EntityDbContext>>();
            var registration = Container.GetRegistration(typeof(IDatabaseInitializer<EntityDbContext>));

            instance.ShouldNotBeNull();
            instance.ShouldBeType<BrownfieldDbInitializer>();
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
        }

        [Fact]
        public void RegistersDbContext_PropertyInjecting_BrownfieldDbInitializer_WhenSettingIsNotGreenfield()
        {
            using (Container.BeginLifetimeScope())
            {
                var dbContext = Container.GetInstance<EntityDbContext>();
                dbContext.Initializer.ShouldNotBeNull();
                dbContext.Initializer.ShouldBeType<BrownfieldDbInitializer>();
            }
        }

        [Fact]
        public void RegistersICustomizeDb_UsingSqlServerDbCustomizer_Transiently_WhenSettingIsGreenfield()
        {
            var container = new Container();
            container.ComposeRoot(new RootCompositionSettings { IsGreenfield = true, });
            var instance = container.GetInstance<ICustomizeDb>();
            var registration = container.GetRegistration(typeof(ICustomizeDb));

            instance.ShouldNotBeNull();
            instance.ShouldBeType<SqlServerScriptsCustomizer>();
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
        }

        [Fact]
        public void RegistersIDbInitializer_UsingGreenfieldDbInitializer_Transiently_WhenSettingIsGreenfield()
        {
            var container = new Container();
            container.ComposeRoot(new RootCompositionSettings { IsGreenfield = true, });
            var instance = container.GetInstance<IDatabaseInitializer<EntityDbContext>>();
            var registration = container.GetRegistration(typeof(IDatabaseInitializer<EntityDbContext>));

            instance.ShouldNotBeNull();
            instance.ShouldBeType<GreenfieldDbInitializer>();
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
        }

        [Fact]
        public void RegistersDbContext_PropertyInjecting_GreenfieldDbInitializer_Transiently_WhenSettingIsGreenfield()
        {
            var container = new Container();
            container.ComposeRoot(new RootCompositionSettings { IsGreenfield = true, });
            using (container.BeginLifetimeScope())
            {
                var dbContext = container.GetInstance<EntityDbContext>();
                dbContext.Initializer.ShouldNotBeNull();
                dbContext.Initializer.ShouldBeType<GreenfieldDbInitializer>();
            }
        }

        [Fact]
        public void RegistersICreateDbModel_UsingDefaulDbtModelCreator_Transiently()
        {
            var instance = Container.GetInstance<ICreateDbModel>();
            var registration = Container.GetRegistration(typeof(ICreateDbModel));

            instance.ShouldNotBeNull();
            instance.ShouldBeType<DefaultDbModelCreator>();
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
        }

        [Fact]
        public void RegistersDbContext_WithScopedLifestyleHybrid_WebRequest_LifetimeScope()
        {
            var registration = Container.GetRegistration(typeof(EntityDbContext));
            registration.Lifestyle.ShouldImplement<ScopedLifestyle>();
            registration.Lifestyle.Name.ShouldEqual("Hybrid Web Request / Lifetime Scope");
        }

        [Fact]
        public void RegistersIUnitOfWork_WithScopedLifestyleHybrid_WebRequest_LifetimeScope()
        {
            var registration = Container.GetRegistration(typeof(IUnitOfWork));
            registration.Lifestyle.ShouldImplement<ScopedLifestyle>();
            registration.Lifestyle.Name.ShouldEqual("Hybrid Web Request / Lifetime Scope");
        }

        [Fact]
        public void RegistersIWriteEntities_WithScopedLifestyleHybrid_WebRequest_LifetimeScope()
        {
            var registration = Container.GetRegistration(typeof(IWriteEntities));
            registration.Lifestyle.ShouldImplement<ScopedLifestyle>();
            registration.Lifestyle.Name.ShouldEqual("Hybrid Web Request / Lifetime Scope");
        }

        [Fact]
        public void RegistersIReadEntities_WithScopedLifestyleHybrid_WebRequest_LifetimeScope()
        {
            var registration = Container.GetRegistration(typeof(IReadEntities));
            registration.Lifestyle.ShouldImplement<ScopedLifestyle>();
            registration.Lifestyle.Name.ShouldEqual("Hybrid Web Request / Lifetime Scope");
        }

        [Fact]
        public void RegistersDbContext_PerWebRequest_WhenHttpContextIsNotNull()
        {
            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://localhost", null), new HttpResponse(null));
            HttpContext.Current.Items.Count.ShouldEqual(0);
            var dbContext = Container.GetInstance<EntityDbContext>();
            dbContext.ShouldNotBeNull();
            HttpContext.Current.Items.Count.ShouldBeGreaterThan(0);
            var items = HttpContext.Current.Items.Cast<DictionaryEntry>().ToArray();
            items.Select(x => x.Value).ShouldContain(dbContext);
            var disposables = items.Select(x => x.Value).SingleOrDefault(x => x is IEnumerable<IDisposable>) as IEnumerable<IDisposable>;
            var disposablesArray = disposables != null ? disposables as IDisposable[] ?? disposables.ToArray() : null;
            disposablesArray.ShouldNotBeNull();
            disposablesArray.ShouldContain(dbContext);
        }

        [Fact]
        public void RegistersInterfaceImplementations_AsSameInstances()
        {
            using (Container.BeginLifetimeScope())
            {
                var dbContext = Container.GetInstance<EntityDbContext>();
                var unitOfWork = Container.GetInstance<IUnitOfWork>();
                var commands = Container.GetInstance<IWriteEntities>();
                var queries = Container.GetInstance<IReadEntities>();

                dbContext.ShouldEqual(unitOfWork);
                dbContext.ShouldEqual(commands);
                dbContext.ShouldEqual(queries);
            }
        }
    }
}
