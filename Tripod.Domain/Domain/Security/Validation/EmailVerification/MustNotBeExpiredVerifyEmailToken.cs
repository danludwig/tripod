using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeExpiredVerifyEmailToken : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBeExpiredVerifyEmailToken(IProcessQueries queries)
            : base(() => Resources.Validation_EmailVerificationTicket_IsExpired)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var token = (string)context.PropertyValue;
            var userToken = _queries.Execute(new EmailVerificationUserToken(token)).Result;
            if (userToken == null) return true;
            var ticket = userToken.Value;

            if (string.IsNullOrWhiteSpace(ticket)) return true;
            var entity = _queries.Execute(new EmailVerificationBy(ticket)).Result;
            if (entity == null) return true;
            if (entity.ExpiresOnUtc >= DateTime.UtcNow) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustNotBeExpiredVerifyEmailTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeExpiredVerifyEmailToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeExpiredVerifyEmailToken(queries));
        }
    }
}
