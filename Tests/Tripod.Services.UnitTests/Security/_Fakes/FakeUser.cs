using Tripod.Domain.Security;

namespace Tripod.Services.Security
{
    public class FakeUser : User
    {
        public FakeUser(int id) { Id = id; }
    }
}
