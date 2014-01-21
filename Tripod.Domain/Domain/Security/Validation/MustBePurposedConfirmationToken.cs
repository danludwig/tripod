using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBePurposedConfirmationToken<T> : PropertyValidator
    {
        private readonly Func<T, EmailConfirmationPurpose> _purpose;
        private readonly IProcessQueries _queries;

        internal MustBePurposedConfirmationToken(Func<T, EmailConfirmationPurpose> purpose, IProcessQueries queries)
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

    public static class MustBePurposedConfirmationTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBePurposedConfirmationToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, EmailConfirmationPurpose> purpose, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustBePurposedConfirmationToken<T>(purpose, queries));
        }
    }
}
