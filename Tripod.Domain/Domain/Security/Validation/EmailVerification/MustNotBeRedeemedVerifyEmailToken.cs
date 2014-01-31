using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeRedeemedVerifyEmailToken : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBeRedeemedVerifyEmailToken(IProcessQueries queries)
            : base(() => Resources.Validation_EmailVerificationTicket_IsRedeemed)
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
            if (!entity.RedeemedOnUtc.HasValue) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustNotBeRedeemedVerifyEmailTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeRedeemedVerifyEmailToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeRedeemedVerifyEmailToken(queries));
        }
    }
}
