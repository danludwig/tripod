namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedEmailVerification : EmailVerification
    {
        protected internal ProxiedEmailVerification()
        {
        }

        protected internal ProxiedEmailVerification(int id)
            : this()
        {
            Id = id;
        }

    public override EmailAddress EmailAddress { get; protected internal set; }
        public override EmailMessage Message { get; protected internal set; }
    }
}