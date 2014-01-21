using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeUnverifiedEmail<T> : PropertyValidator
    {
        private readonly Func<T, string> _token;
        private readonly IProcessQueries _queries;

        internal MustNotBeUnverifiedEmail(Func<T, string> token, IProcessQueries queries)
            : base(() => Resources.Validation_EmailConfirmationTicket_IsWrongPurpose)
        {
            if (token == null) throw new ArgumentNullException("token");
            if (queries == null) throw new ArgumentNullException("queries");
            _token = token;
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var userName = (string)context.PropertyValue;
            var token = _token((T)context.Instance);
            //if (string.IsNullOrWhiteSpace(ticket)) return true;
            //var entity = _queries.Execute(new EmailConfirmationBy(ticket)).Result;
            //if (entity == null) return true;
            //return entity.Purpose == purpose;
            return false;
        }
    }

    public static class MustNotBeUnverifiedEmailExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeUnverifiedEmail<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, string> token, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeUnverifiedEmail<T>(token, queries));
        }
    }
}
