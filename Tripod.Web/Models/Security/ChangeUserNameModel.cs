using Tripod.Domain.Security;

namespace Tripod.Web.Models
{
    public class ChangeUserNameModel : UserViewModelBase
    {
        public ChangeUserName Command { get; set; }
    }
}