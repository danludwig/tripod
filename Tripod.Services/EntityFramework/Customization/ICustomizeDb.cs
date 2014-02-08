using System.Data.Entity;

namespace Tripod.Services.EntityFramework
{
    public interface ICustomizeDb
    {
        void Customize(DbContext dbContext);
    }
}
