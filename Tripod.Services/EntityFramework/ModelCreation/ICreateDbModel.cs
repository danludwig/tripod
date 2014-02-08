using System.Data.Entity;

namespace Tripod.Services.EntityFramework
{
    public interface ICreateDbModel
    {
        void Create(DbModelBuilder modelBuilder);
    }
}
