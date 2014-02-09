using Tripod.Domain.Security;

namespace Tripod.Web.Models
{
    public class CreateLocalMembershipModel : UserViewModelBase
    {
        public CreateLocalMembership Command { get; set; }
    }
}