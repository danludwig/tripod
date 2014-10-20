using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustBeValidVerifyEmailTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValidVerifyEmailToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, string> ticket)
        {
            return ruleBuilder.SetValidator(new MustBeValidVerifyEmailToken<T>(queries, ticket));
        }
    }

    internal class MustBeValidVerifyEmailToken<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, string> _ticket;

        internal MustBeValidVerifyEmailToken(IProcessQueries queries, Func<T, string> ticket)
            : base(() => Resources.Validation_DoesNotExist_NoValue)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (ticket == null) throw new ArgumentNullException("ticket");
            _queries = queries;
            _ticket = ticket;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var token = (string)context.PropertyValue;
            var ticket = _ticket((T)context.Instance);

            // there should be another validator which detects emptiness of ticket
            if (string.IsNullOrWhiteSpace(ticket)) return true;

            var verification = _queries.Execute(new EmailVerificationBy(ticket)).Result;

            // there should be another validator which detects the presence of the entity by ticket
            if (verification == null) return true;

            // token is not valid unless it matches the ticket's token
            if (verification.Token != token) return false;

            var query = new EmailVerificationTokenIsValid(token, ticket, verification.Purpose);
            var isValid = _queries.Execute(query).Result;
            return isValid;
        }
    }
}
