using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;

namespace Tripod.Services.EntityFramework
{
    public class DefaultDbModelCreator : ICreateDbModel
    {
        public void Create(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            var assembly = Assembly.GetAssembly(GetType());
            var typesToRegister = assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(StructuralTypeConfiguration<>).IsGenericallyAssignableFrom(t))
                .ToArray();
            foreach (var configurationInstance in typesToRegister.Select(Activator.CreateInstance))
                modelBuilder.Configurations.Add((dynamic)configurationInstance);
        }
    }
}
