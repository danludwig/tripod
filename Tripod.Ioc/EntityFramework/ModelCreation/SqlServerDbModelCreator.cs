using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;

namespace Tripod.Ioc.EntityFramework
{
    public class SqlServerDbModelCreator : ICreateDbModel
    {
        public void Create(DbModelBuilder modelBuilder, Assembly assembly)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            var complexType = typeof(ComplexTypeConfiguration<>);
            var entityType = typeof(EntityTypeConfiguration<>);

            assembly = assembly ?? Assembly.GetAssembly(GetType());
            var typesToRegister = assembly.GetTypes()
                .Where(t => !t.IsAbstract &&
                (
                       complexType.IsGenericallyAssignableFrom(t)
                    || entityType.IsGenericallyAssignableFrom(t)
                ))
                .ToArray();
            foreach (var typeToRegister in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(typeToRegister);
                modelBuilder.Configurations.Add(configurationInstance);
            }
        }
    }
}
