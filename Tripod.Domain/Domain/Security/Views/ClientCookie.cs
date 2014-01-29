namespace Tripod.Domain.Security
{
    public class ClientCookie
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string GravatarHash { get; set; }
    }
}
