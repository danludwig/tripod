using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeVerifiedConfirmationSecret<T> : PropertyValidator
    {
        private readonly Func<T, string> _ticket;
        private readonly IProcessQueries _queries;

        internal MustBeVerifiedConfirmationSecret(Func<T, string> ticket, IProcessQueries queries)
            : base(() => Resources.Validation_EmailConfirmationSecret_IsWrong)
        {
            if (ticket == null) throw new ArgumentNullException("ticket");
            if (queries == null) throw new ArgumentNullException("queries");
            _ticket = ticket;
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var secret = (string)context.PropertyValue;
            var ticket = _ticket((T)context.Instance);
            if (string.IsNullOrWhiteSpace(secret)) return true;
            var entity = _queries.Execute(new EmailConfirmationBy(ticket)).Result;
            if (entity == null) return true;
            if (entity.Secret == secret) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustBeVerifiedConfirmationSecretExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeVerifiedConfirmationSecret<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, string> ticket, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustBeVerifiedConfirmationSecret<T>(ticket, queries));
        }
    }
}
