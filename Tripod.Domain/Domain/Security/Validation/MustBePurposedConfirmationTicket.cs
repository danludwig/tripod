using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBePurposedConfirmationTicket<T> : PropertyValidator
    {
        private readonly Func<T, EmailConfirmationPurpose> _purpose;
        private readonly IProcessQueries _queries;

        internal MustBePurposedConfirmationTicket(Func<T, EmailConfirmationPurpose> purpose, IProcessQueries queries)
            : base(() => Resources.Validation_EmailConfirmationTicket_IsWrongPurpose)
        {
            if (purpose == null) throw new ArgumentNullException("purpose");
            if (queries == null) throw new ArgumentNullException("queries");
            _purpose = purpose;
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ticket = (string)context.PropertyValue;
            var purpose = _purpose((T)context.Instance);
            if (string.IsNullOrWhiteSpace(ticket)) return true;
            var entity = _queries.Execute(new EmailConfirmationBy(ticket)).Result;
            if (entity == null) return true;
            return entity.Purpose == purpose;
        }
    }

    public static class MustBePurposedConfirmationTicketExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBePurposedConfirmationTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, EmailConfirmationPurpose> purpose, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustBePurposedConfirmationTicket<T>(purpose, queries));
        }
    }
}
