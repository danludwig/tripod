namespace Tripod.Domain.Security
{
    public class EmailAddress : EntityWithId<int>
    {
        protected internal EmailAddress() { }

        public int? OwnerId { get; protected internal set; }
        public virtual User Owner { get; protected internal set; }

        public string Value { get; protected internal set; }
        public bool IsDefault { get; protected internal set; }
        public bool IsConfirmed { get; protected internal set; }

        public static class Constraints
        {
            public const string Label = "Email address";

            public const string ValueLabel = Label;
            public const int ValueMaxLength = 255;
        }
    }
}
