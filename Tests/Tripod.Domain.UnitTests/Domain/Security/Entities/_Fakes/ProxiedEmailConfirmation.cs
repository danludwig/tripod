namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedEmailConfirmation : EmailConfirmation
    {
        public override EmailAddress Owner { get; protected internal set; }
        public override EmailMessage Message { get; protected internal set; }
    }
}