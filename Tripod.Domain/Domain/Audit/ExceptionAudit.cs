using System;

namespace Tripod.Domain.Audit
{
    public class ExceptionAudit : EntityWithId<Guid>
    {
        protected ExceptionAudit()
        {
        }

        public string Application { get; protected set; }
        public string Host { get; protected set; }
        public string Type { get; protected set; }
        public string Source { get; protected set; }
        public string Message { get; protected set; }
        public string User { get; protected set; }
        public int StatusCode { get; protected set; }
        public DateTime OnUtc { get; protected set; }
        public int Sequence { get; protected set; }
        public string Xml { get; protected set; }

        public static class Constraints
        {
            public const string Label = "Error";

            public const string ApplicationLabel = "Application name";
            public const int ApplicationMaxLength = 60;

            public const string HostLabel = "Host";
            public const int HostMaxLength = 50;

            public const string TypeLabel = "Type";
            public const int TypeMaxLength = 100;

            public const string SourceLabel = "Type";
            public const int SourceMaxLength = 60;

            public const string MessageLabel = "Message";
            public const int MessageMaxLength = 500;

            public const string UserLabel = "User";
            public const string StatusCodeLabel = "Status code";
            public const string OnUtcLabel = "Date & time (UTC)";
            public const string SequenceLabel = "Sequence number";
            public const string XmlLabel = "Error XML";
        }
    }
}
