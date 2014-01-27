using System;
using System.Linq.Expressions;

namespace Tripod.Domain.Security
{
    public class UserView
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string DefaultEmailAddress { get; set; }
        public string DefaultEmailHash { get; set; }

        internal static Expression<Func<User, object>>[] EagerLoad = new Expression<Func<User, object>>[]
        {
            x => x.EmailAddresses,
        }; 
    }
}
