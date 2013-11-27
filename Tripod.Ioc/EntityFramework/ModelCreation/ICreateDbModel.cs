using System.Data.Entity;
using System.Reflection;

namespace Tripod.Ioc.EntityFramework
{
    public interface ICreateDbModel
    {
        void Create(DbModelBuilder modelBuilder, Assembly assembly);
    }
}
