using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustNotBeExpiredVerifyEmailTicketExtensions
    {
        /// <summary>
        /// Validates that the EmailVerification with this Ticket is not expired.
        /// </summary>
        /// <typeparam name="T">The command with the EmailVerification Ticket to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating EmailVerification by Ticket.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustNotBeExpiredVerifyEmailTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeExpiredVerifyEmailTicket(queries));
        }
    }

    internal class MustNotBeExpiredVerifyEmailTicket : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBeExpiredVerifyEmailTicket(IProcessQueries queries)
            : base(() => Resources.Validation_EmailVerificationTicket_IsExpired)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ticket = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(ticket)) return true;
            var entity = _queries.Execute(new EmailVerificationBy(ticket)).Result;
            if (entity == null) return true;
            if (entity.ExpiresOnUtc >= DateTime.UtcNow) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }
}
