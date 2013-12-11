using Xunit;

namespace Tripod.Ioc.EntityFramework
{
    public class VanillaDbCustomizerTests
    {
        [Fact]
        public void DoesNotCustomizeAnything()
        {
            var dbCustomizer = new VanillaDbCustomizer();
            dbCustomizer.Customize(null);
        }
    }
}
