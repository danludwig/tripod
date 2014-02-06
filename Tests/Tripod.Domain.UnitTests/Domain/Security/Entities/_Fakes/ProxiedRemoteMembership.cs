namespace Tripod.Domain.Security
{
    public class ProxiedRemoteMembership : RemoteMembership
    {
        protected internal ProxiedRemoteMembership(string loginProvider, string providerKey)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }

        public override User User { get; protected internal set; }
    }
}