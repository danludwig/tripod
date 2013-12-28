using Microsoft.AspNet.Identity;

namespace Tripod
{
    public class RemoteMembershipTicket
    {
        public string UserName { get; set; }
        public UserLoginInfo Login { get; set; }
    }
}