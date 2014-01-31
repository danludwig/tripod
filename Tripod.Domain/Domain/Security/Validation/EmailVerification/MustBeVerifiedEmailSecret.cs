using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeVerifiedEmailSecret<T> : PropertyValidator
    {
        private readonly Func<T, string> _ticket;
        private readonly IProcessQueries _queries;

        internal MustBeVerifiedEmailSecret(Func<T, string> ticket, IProcessQueries queries)
            : base(() => Resources.Validation_EmailVerificationSecret_IsWrong)
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
            var entity = _queries.Execute(new EmailVerificationBy(ticket)).Result;
            if (entity == null) return true;
            if (entity.Secret == secret) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustBeVerifiedEmailSecretExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeVerifiedEmailSecret<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, string> ticket, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustBeVerifiedEmailSecret<T>(ticket, queries));
        }
    }
}
