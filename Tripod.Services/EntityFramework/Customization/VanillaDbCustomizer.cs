using System.Data.Entity;

namespace Tripod.Services.EntityFramework
{
    public class VanillaDbCustomizer : ICustomizeDb
    {
        public void Customize(DbContext dbContext)
        {
            // do not customize
        }
    }
}
