using System.Data.Entity;
using System.Web;
using SimpleInjector;
using SimpleInjector.Extensions.LifetimeScoping;
using SimpleInjector.Integration.Web;

namespace Tripod.Ioc.EntityFramework
{
    public static class CompositionRoot
    {
        public static void RegisterEntityFramework(this Container container, bool isGreenfield = false)
        {
            if (isGreenfield)
            {
                container.Register<ICustomizeDb, VanillaDbCustomizer>();
                container.Register<IDatabaseInitializer<EntityDbContext>, GreenfieldDbInitializer>();
            }
            else
            {
                container.Register<ICustomizeDb, VanillaDbCustomizer>();
                container.Register<IDatabaseInitializer<EntityDbContext>, BrownfieldDbInitializer>();
            }

            container.Register<ICreateDbModel, DefaultDbModelCreator>();
            container.RegisterInitializer<EntityDbContext>(container.InjectProperties);

            // register the lifestyle
            var lifestyle = Lifestyle.CreateHybrid(
                lifestyleSelector: () => HttpContext.Current != null,
                trueLifestyle: new WebRequestLifestyle(),
                falseLifestyle: new LifetimeScopeLifestyle()
            );

            // register the db context & its 3 interfaces
            var contextRegistration = lifestyle.CreateRegistration<EntityDbContext, EntityDbContext>(container);
            container.AddRegistration(typeof(EntityDbContext), contextRegistration);
            container.AddRegistration(typeof(IUnitOfWork), contextRegistration);
            container.AddRegistration(typeof(ICommandEntities), contextRegistration);
            container.AddRegistration(typeof(IQueryEntities), contextRegistration);
        }
    }
}
