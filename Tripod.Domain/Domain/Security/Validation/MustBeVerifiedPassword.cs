using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeVerifiedPassword<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, string> _userName;

        internal MustBeVerifiedPassword(IProcessQueries queries, Func<T, string> userName)
            : base(() => Resources.Validation_InvalidPassword)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (userName == null) throw new ArgumentNullException("userName");
            _queries = queries;
            _userName = userName;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var password = (string)context.PropertyValue;
            var userName = _userName((T)context.Instance);
            var query = new IsPasswordVerified
            {
                UserNameOrVerifiedEmail = userName,
                Password = password,
            };
            var isVerified = _queries.Execute(query).Result;

            return isVerified;
        }
    }

    public static class MustBeVerifiedPasswordExtensions
    {
        /// <summary>
        /// Validates that this is the verified password for the user with name provided in the argument.
        /// </summary>
        /// <typeparam name="T">The command with the Password to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for verifying password.</param>
        /// <param name="userName">Name property of the User to verify this password for.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustBeVerifiedPassword<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, string> userName)
        {
            return ruleBuilder.SetValidator(new MustBeVerifiedPassword<T>(queries, userName));
        }
    }
}
