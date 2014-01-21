using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeValidConfirmEmailToken : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustBeValidConfirmEmailToken(IProcessQueries queries)
            : base(() => Resources.Validation_DoesNotExist_NoValue)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var token = (string)context.PropertyValue;
            var userToken = _queries.Execute(new EmailConfirmationUserToken(token)).Result;
            return userToken != null;
        }
    }

    public static class MustBeValidConfirmEmailTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValidConfirmEmailToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustBeValidConfirmEmailToken(queries));
        }
    }
}
