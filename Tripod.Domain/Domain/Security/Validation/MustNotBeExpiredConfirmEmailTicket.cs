using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeExpiredConfirmEmailTicket : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBeExpiredConfirmEmailTicket(IProcessQueries queries)
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
            if (entity.ExpiresOnUtc >= DateTime.UtcNow) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustNotBeExpiredConfirmEmailTicketExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeExpiredConfirmEmailTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeExpiredConfirmEmailTicket(queries));
        }
    }
}
