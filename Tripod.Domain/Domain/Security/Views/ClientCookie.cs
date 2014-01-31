namespace Tripod.Domain.Security
{
    public class ClientCookie
    {
        public int UserId { [UsedImplicitly] get; set; }
        public string UserName { [UsedImplicitly] get; set; }
        public string GravatarHash { get; set; }
    }
}
