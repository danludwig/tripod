using Xunit;

namespace Tripod.Services.EntityFramework
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
