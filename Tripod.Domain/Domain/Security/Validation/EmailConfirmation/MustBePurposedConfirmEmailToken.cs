using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBePurposedConfirmEmailToken<T> : PropertyValidator
    {
        private readonly Func<T, EmailConfirmationPurpose> _purpose;
        private readonly IProcessQueries _queries;

        internal MustBePurposedConfirmEmailToken(Func<T, EmailConfirmationPurpose> purpose, IProcessQueries queries)
            : base(() => Resources.Validation_EmailConfirmationTicket_IsWrongPurpose)
        {
            if (purpose == null) throw new ArgumentNullException("purpose");
            if (queries == null) throw new ArgumentNullException("queries");
            _purpose = purpose;
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var token = (string)context.PropertyValue;
            var userToken = _queries.Execute(new EmailConfirmationUserToken(token)).Result;
            if (userToken == null) return true;
            var ticket = userToken.Value;

            var purpose = _purpose((T)context.Instance);
            if (string.IsNullOrWhiteSpace(ticket)) return true;
            var entity = _queries.Execute(new EmailConfirmationBy(ticket)).Result;
            if (entity == null) return true;
            if (entity.Purpose == purpose) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustBePurposedConfirmEmailTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBePurposedConfirmEmailToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, EmailConfirmationPurpose> purpose, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustBePurposedConfirmEmailToken<T>(purpose, queries));
        }
    }
}
