namespace Tripod.Domain.Security
{
    [UsedImplicitly]
    public class ProxiedEmailMessage : EmailMessage
    {
        protected internal ProxiedEmailMessage() { }

        protected internal ProxiedEmailMessage(int id)
            : this()
        {
            Id = id;
        }

    public override EmailAddress EmailAddress { get; protected internal set; }
    }
}