namespace Tripod.Domain.Security
{
    public class UserView
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PrimaryEmailAddress { get; set; }
        public string PrimaryEmailHash { get; set; }
    }
}
