using System;

namespace Tripod.Domain.Security
{
    public class EmailMessage : EntityWithId<int>
    {
        protected internal EmailMessage() { }

        public int OwnerId { get; protected internal set; }
        public virtual EmailAddress Owner { get; protected internal set; }

        public string From { get; protected internal set; }
        public string Subject { get; protected internal set; }
        public string Body { get; protected internal set; }
        public bool IsBodyHtml { get; protected internal set; }
        public DateTime SendOnUtc { get; protected internal set; }
        public DateTime? SentOnUtc { get; protected internal set; }

        public static class Constraints
        {
            public const string Label = "Email message";

            public const string FromLabel = "From";
            public const int FromMaxLength = 250;
            public const int SubjectMaxLength = 150;
        }
    }
}