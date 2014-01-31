using System;

namespace Tripod.Domain.Security
{
    public class EmailVerification : EntityWithId<int>
    {
        protected internal EmailVerification() { }

        public int OwnerId { get; [UsedImplicitly] protected internal set; }
        public virtual EmailAddress Owner { get; protected internal set; }

        public string Token { get; protected internal set; }
        public string Ticket { get; protected internal set; }
        public string Secret { get; protected internal set; }
        public DateTime ExpiresOnUtc { get; protected internal set; }
        public DateTime? RedeemedOnUtc { get; protected internal set; }
        public EmailVerificationPurpose Purpose { get; protected internal set; }

        public virtual EmailMessage Message { get; protected internal set; }

        public static class Constraints
        {
            public const string Label = "Email verification";
            public const string SecretLabel = "Secret code";
            public const int SecretMaxLength = 50;
            public const string TicketLabel = "Verification ticket";
            public const int TicketMaxLength = 100;
        }
    }
}