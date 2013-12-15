using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;

namespace Tripod.Ioc.EntityFramework
{
    public class DefaultDbModelCreator : ICreateDbModel
    {
        public void Create(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //var complexType = typeof(ComplexTypeConfiguration<>);
            var structuralType = typeof(StructuralTypeConfiguration<>);

            var assembly = Assembly.GetAssembly(GetType());
            var typesToRegister = assembly.GetTypes()
                .Where(t => !t.IsAbstract &&
                (
                    //complexType.IsGenericallyAssignableFrom(t) ||
                    structuralType.IsGenericallyAssignableFrom(t)
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
