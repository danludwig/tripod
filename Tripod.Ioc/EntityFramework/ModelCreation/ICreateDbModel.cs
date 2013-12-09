using System.Data.Entity;

namespace Tripod.Ioc.EntityFramework
{
    public interface ICreateDbModel
    {
        void Create(DbModelBuilder modelBuilder);
    }
}
