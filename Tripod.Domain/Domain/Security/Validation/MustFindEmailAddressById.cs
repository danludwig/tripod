using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustFindEmailAddressById : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindEmailAddressById(IProcessQueries queries)
            : base(() => Resources.Validation_DoesNotExist_IntIdValue)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var emailAddressId = (int)context.PropertyValue;
            var entity = _queries.Execute(new EmailAddressBy(emailAddressId)).Result;
            return entity != null;
        }
    }

    public static class MustFindEmailAddressByIdExtensions
    {
        public static IRuleBuilderOptions<T, int> MustFindEmailAddressById<T>
            (this IRuleBuilder<T, int> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindEmailAddressById(queries));
        }
    }
}
