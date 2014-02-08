using Should;
using Xunit;

namespace Tripod.Services.Net
{
    public class SmtpMailMessageDeliveryTests
    {
        [Fact]
        public void HasNoArgCtor()
        {
            var instance = new SmtpMailMessageDelivery();
            instance.ShouldNotBeNull();
        }
    }
}
