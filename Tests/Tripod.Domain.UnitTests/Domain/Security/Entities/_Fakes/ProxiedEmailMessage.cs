namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedEmailMessage : EmailMessage
    {
        public override EmailAddress Owner { get; protected internal set; }
    }
}