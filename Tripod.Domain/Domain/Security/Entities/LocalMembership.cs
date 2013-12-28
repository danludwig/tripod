namespace Tripod.Domain.Security
{
    public class LocalMembership : EntityWithId<int>
    {
        protected internal LocalMembership() { }

        public virtual User Owner { get; protected internal set; }

        public string PasswordHash { get; protected internal set; }

        public bool IsConfirmed { get; protected internal set; }

        public static class Constraints
        {
            public const string Label = "Local password";
            public const string PasswordLabel = "Password";
            public const string Password2Label = "Confirm password";
            public const int PasswordMinLength = 8;

            public const int ConfirmationTokenMaxLength = 128;
            public const int PasswordMaxLength = 100;
            public const int PasswordSaltMaxLength = 128;
            public const int PasswordVerificationTokenMaxLength = 128;
        }
    }
}
