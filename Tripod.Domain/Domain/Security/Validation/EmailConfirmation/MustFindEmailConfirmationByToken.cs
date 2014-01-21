using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustFindEmailConfirmationByToken : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindEmailConfirmationByToken(IProcessQueries queries)
            : base(() => Resources.Validation_DoesNotExist_NoValue)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var token = (string)context.PropertyValue;
            var userToken = _queries.Execute(new EmailConfirmationUserToken(token)).Result;
            if (userToken == null) return true;
            var ticket = userToken.Value;
            if (string.IsNullOrWhiteSpace(ticket)) return false;

            var entity = _queries.Execute(new EmailConfirmationBy(ticket)).Result;
            return entity != null;
        }
    }

    public static class MustFindEmailConfirmationByTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustFindEmailConfirmationByToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindEmailConfirmationByToken(queries));
        }
    }
}
