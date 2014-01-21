using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeExpiredConfirmationTicket : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBeExpiredConfirmationTicket(IProcessQueries queries)
            : base(() => Resources.Validation_EmailConfirmationTicket_IsExpired)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ticket = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(ticket)) return true;
            var entity = _queries.Execute(new EmailConfirmationBy(ticket)).Result;
            if (entity == null) return true;
            return entity.ExpiresOnUtc >= DateTime.UtcNow;
        }
    }

    public static class MustNotBeExpiredConfirmationTicketExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeExpiredConfirmationTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeExpiredConfirmationTicket(queries));
        }
    }
}
