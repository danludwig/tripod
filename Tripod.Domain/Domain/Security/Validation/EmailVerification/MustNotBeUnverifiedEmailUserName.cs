using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustNotBeUnverifiedEmailUserNameExtensions
    {
        /// <summary>
        /// Validates that this candidate User Name is not an unverified EmailAddress.
        /// </summary>
        /// <typeparam name="T">Command with the candidate User Name text.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating EmailVerification by Ticket.</param>
        /// <param name="ticket">EmailVerification Ticket that allows usage of this email address as a User Name.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustNotBeUnverifiedEmailUserName<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, string> ticket)
        {
            return ruleBuilder.SetValidator(new MustNotBeUnverifiedEmailUserName<T>(queries, ticket));
        }

        /// <summary>
        /// Validates that this candidate User Name is not an unverified EmailAddress.
        /// </summary>
        /// <typeparam name="T">Command with the candidate User Name text.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating EmailAddress by User Id.</param>
        /// <param name="userId">Id of the User expected to own the possible email address User Name candidate.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustNotBeUnverifiedEmailUserName<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, int> userId)
        {
            return ruleBuilder.SetValidator(new MustNotBeUnverifiedEmailUserName<T>(queries, userId));
        }
    }

    internal class MustNotBeUnverifiedEmailUserName<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, string> _ticket;
        private readonly Func<T, int> _userId;

        private MustNotBeUnverifiedEmailUserName(IProcessQueries queries)
            : base(() => Resources.Validation_UserName_AllowedEmailAddress)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        internal MustNotBeUnverifiedEmailUserName(IProcessQueries queries, Func<T, string> ticket)
            : this(queries)
        {
            if (ticket == null) throw new ArgumentNullException("ticket");
            _ticket = ticket;
        }

        internal MustNotBeUnverifiedEmailUserName(IProcessQueries queries, Func<T, int> userId)
            : this(queries)
        {
            if (userId == null) throw new ArgumentNullException("userId");
            _userId = userId;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var userName = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(userName)) return true;

            if (!EmailAddress.ValueRegex.IsMatch(userName)) return true;

            if (_ticket != null)
            {
                var ticket = _ticket((T)context.Instance);
                var verification = _queries.Execute(new EmailVerificationBy(ticket)
                {
                    EagerLoad = new Expression<Func<EmailVerification, object>>[]
                    {
                        x => x.EmailAddress,
                    },
                }).Result;
                if (verification == null) return true;
                if (verification.EmailAddress.Value.Equals(userName, StringComparison.OrdinalIgnoreCase)) return true;
            }
            else
            {
                var userId = _userId((T)context.Instance);
                var emailAddresses = _queries.Execute(new EmailAddressesBy(userId)
                {
                    IsVerified = true,
                }).Result;
                var matchingAddress = emailAddresses.ByValueAsync(userName).Result;
                if (matchingAddress != null) return true;
            }

            context.MessageFormatter.AppendArgument("PropertyName", User.Constraints.NameLabel.ToLower());
            return false;
        }
    }
}
