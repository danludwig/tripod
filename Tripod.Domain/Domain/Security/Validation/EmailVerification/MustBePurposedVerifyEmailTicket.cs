using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBePurposedVerifyEmailTicket<T> : PropertyValidator
    {
        private readonly Func<T, EmailVerificationPurpose> _purpose;
        private readonly IProcessQueries _queries;

        internal MustBePurposedVerifyEmailTicket(Func<T, EmailVerificationPurpose> purpose, IProcessQueries queries)
            : base(() => Resources.Validation_EmailVerificationTicket_IsWrongPurpose)
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
            var entity = _queries.Execute(new EmailVerificationBy(ticket)).Result;
            if (entity == null) return true;
            if (entity.Purpose == purpose) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustBePurposedVerifyEmailTicketExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBePurposedVerifyEmailTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, EmailVerificationPurpose> purpose, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustBePurposedVerifyEmailTicket<T>(purpose, queries));
        }
    }
}
