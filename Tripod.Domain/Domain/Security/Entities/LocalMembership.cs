namespace Tripod.Domain.Security
{
    public class LocalMembership : EntityWithId<int>
    {
        protected internal LocalMembership() { }

        public virtual User Owner { get; protected internal set; }

        public string PasswordHash { get; protected internal set; }

        public static class Constraints
        {
            public const string Label = "Local password";
            public const string PasswordLabel = "Password";
            public const string PasswordConfirmationLabel = "Password confirmation";
            public const int PasswordMinLength = 8;
            public const int PasswordMaxLength = 100;
            public const string OldPasswordLabel = "Old password";
            public const string NewPasswordLabel = "New password";
            public const string NewPasswordConfirmationLabel = "New password confirmation";

        }
    }
}
