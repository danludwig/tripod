using System.Data.Entity;

namespace Tripod.Ioc.EntityFramework
{
    public interface ICustomizeDb
    {
        void Customize(DbContext db);
    }
}
