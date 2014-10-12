using System.Collections.Generic;
using Tripod.Domain.Security;

namespace Tripod.Web.Models
{
    public class LoginSettingsModel : UserViewModelBase
    {
        public IList<RemoteMembershipView> Logins { get; set; }
        public bool IsDeleteAllowed { get; set; }
    }
}