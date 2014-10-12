using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace Tripod
{
    public static class IdentityExtensions
    {
        public static bool HasAppUserId(this IIdentity identity)
        {
            return !string.IsNullOrWhiteSpace(identity.GetUserId());
        }
    }
}
