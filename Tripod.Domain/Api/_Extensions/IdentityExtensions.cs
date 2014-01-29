using System;
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

        public static int GetAppUserId(this IIdentity identity)
        {
            var userIdString = identity.GetUserId();
            int userIdInt;
            if (int.TryParse(userIdString, out userIdInt))
            {
                return userIdInt;
            }
            throw new InvalidOperationException("The application identity's user ID was expected to be an integer.");
        }
    }
}
