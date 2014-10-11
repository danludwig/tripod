using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;

namespace Tripod.Services.Security
{
    [UsedImplicitly]
    public class UserTokenSecurityStore : IUserStore<UserTicket, string>
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(UserTicket userTicket)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UserTicket userTicket)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(UserTicket userTicket)
        {
            throw new NotImplementedException();
        }

        public Task<UserTicket> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserTicket> FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }
    }
}