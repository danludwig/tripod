using System;

namespace Tripod.Domain.Audit
{
    public class ExceptionAudit : EntityWithId<Guid>
    {
        protected ExceptionAudit()
        {
        }

        public string Application { get; [UsedImplicitly] protected set; }
        public string Host { get; [UsedImplicitly] protected set; }
        public string Type { get; [UsedImplicitly] protected set; }
        public string Source { get; [UsedImplicitly] protected set; }
        public string Message { get; [UsedImplicitly] protected set; }
        public string User { get; [UsedImplicitly] protected set; }
        [UsedImplicitly]
        public int StatusCode { get; protected set; }
        public DateTime OnUtc { get; [UsedImplicitly] protected set; }
        public int Sequence { get; [UsedImplicitly] protected set; }
        public string Xml { get; [UsedImplicitly] protected set; }

        public static class Constraints
        {
            [UsedImplicitly]
            public const string Label = "Error";

            [UsedImplicitly]
            public const string ApplicationLabel = "Application name";
            public const int ApplicationMaxLength = 60;

            [UsedImplicitly]
            public const string HostLabel = "Host";
            public const int HostMaxLength = 50;

            [UsedImplicitly]
            public const string TypeLabel = "Type";
            public const int TypeMaxLength = 100;

            [UsedImplicitly]
            public const string SourceLabel = "Type";
            public const int SourceMaxLength = 60;

            [UsedImplicitly]
            public const string MessageLabel = "Message";
            public const int MessageMaxLength = 500;

            [UsedImplicitly]
            public const string UserLabel = "User";
            [UsedImplicitly]
            public const string StatusCodeLabel = "Status code";
            [UsedImplicitly]
            public const string OnUtcLabel = "Date & time (UTC)";
            [UsedImplicitly]
            public const string SequenceLabel = "Sequence number";
            [UsedImplicitly]
            public const string XmlLabel = "Error XML";
        }
    }
}
