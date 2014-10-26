using Microsoft.AspNet.Identity;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class UserTicketTests
    {
        [Fact]
        public void Id_ReturnsUserName()
        {
            var userTicket = new UserTicket
            {
                UserName = FakeData.String(),
            };
            userTicket.Id.ShouldEqual(userTicket.UserName);
        }

        [Fact]
        public void Implements_IUser()
        {
            var user = new UserTicket() as IUser<string>;
            user.ShouldNotBeNull();
            user.UserName = "test";
            user.UserName.ShouldEqual("test");
        }
    }
}