using Should;
using Xunit;

namespace Tripod.Ioc.Net
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
