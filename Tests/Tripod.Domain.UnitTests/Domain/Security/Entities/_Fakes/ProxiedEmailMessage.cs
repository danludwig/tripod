namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedEmailMessage : EmailMessage
    {
        public override EmailAddress EmailAddress { get; protected internal set; }
    }
}