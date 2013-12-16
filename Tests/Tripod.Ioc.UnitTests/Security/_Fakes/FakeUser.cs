using Tripod.Domain.Security;

namespace Tripod.Ioc.Security
{
    public class FakeUser : User
    {
        public FakeUser(int id) { Id = id; }
    }
}
