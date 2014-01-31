namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedEmailVerification : EmailVerification
    {
        public override EmailAddress Owner { get; protected internal set; }
        public override EmailMessage Message { get; protected internal set; }
    }
}