namespace Tripod.Domain.Security
{
    public class User : EntityWithId<int>
    {
        protected User() { }

        protected internal User(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }

        public static class Constraints
        {
            public const string Label = "User";

            public const string NameLabel = "User name";
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;
        }
    }
}
