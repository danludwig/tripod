using System.Linq;
using Tripod.Domain.Security;
using Xunit;

namespace Tripod
{
    public class EntityExtensionTests
    {
        [Fact]
        public void SingleOrDefaultAsync_UsesSingleOrDefault_WhenQueryableIsNotEntitySet()
        {
            var queryable = new User[0].AsQueryable();
            var result = queryable.SingleOrDefaultAsync(x => x != null).Result;
            Assert.Null(result);
        }
    }
}
