using Tripod.Domain.Security;

namespace Tripod.Web.Models
{
    public class ChangeLocalPasswordModel : UserViewModelBase
    {
        public ChangeLocalPassword Command { get; set; }
    }
}