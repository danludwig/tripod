namespace Tripod.Domain.Guestbook
{
    public class GuestbookEntry : EntityWithId<int>
    {
        protected internal GuestbookEntry() { }

        public string Text { get; protected internal set; }

        public static class Constraints
        {
            public const int TextMaxLength = 200;
        }
    }
}
