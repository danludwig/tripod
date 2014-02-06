using System;

namespace Tripod.Domain.Security
{
    public class EmailMessage : EntityWithId<int>
    {
        protected internal EmailMessage() { }

        public int EmailAddressId { get; [UsedImplicitly] protected internal set; }
        public virtual EmailAddress EmailAddress { get; protected internal set; }

        public string From { get; protected internal set; }
        public string Subject { get; protected internal set; }
        public string Body { get; protected internal set; }
        public bool IsBodyHtml { get; protected internal set; }
        public DateTime SendOnUtc { get; protected internal set; }
        public DateTime? SentOnUtc { get; protected internal set; }
        public DateTime? CancelledOnUtc { [UsedImplicitly] get; protected internal set; }
        public string LastSendError { get; protected internal set; }

        public static class Constraints
        {
            [UsedImplicitly] public const string Label = "Email message";

            [UsedImplicitly] public const string FromLabel = "From";
            public const int FromMaxLength = 250;
            public const int SubjectMaxLength = 150;
            public const int LastSendErrorMaxLength = 500;
        }
    }
}