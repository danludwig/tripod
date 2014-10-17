namespace Tripod.Domain.Security
{
    public class UserWithSpecifiedId : User
    {
        public UserWithSpecifiedId(int id)
        {
            Id = id;
        }
    }
}
