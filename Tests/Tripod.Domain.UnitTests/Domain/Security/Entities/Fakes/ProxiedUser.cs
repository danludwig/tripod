namespace Tripod.Domain.Security
{
    public class ProxiedUser : User
    {
        protected internal ProxiedUser()
        {
            Name = "nameFromDb";
        }
    }
}