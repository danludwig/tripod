using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class UserTicket : IUser<string>
    {
        public string Id { get { return UserName; } }

        public string UserName { get; set; }
    }
}