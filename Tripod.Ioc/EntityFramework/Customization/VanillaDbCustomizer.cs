using System.Data.Entity;

namespace Tripod.Ioc.EntityFramework
{
    public class VanillaDbCustomizer : ICustomizeDb
    {
        public void Customize(DbContext dbContext)
        {
            // do not customize
        }
    }
}
