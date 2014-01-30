using System.Collections.Generic;
using Tripod.Domain.Security;

namespace Tripod.Web.Models
{
    public class EmailAddressSettingsModel : UserViewModelBase
    {
        public IList<EmailAddressView> EmailAddresses { get; set; }
    }
}