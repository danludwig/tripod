using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustNotBePrimaryEmailAddressExtensions
    {
        public static IRuleBuilderOptions<T, int> MustNotBePrimaryEmailAddress<T>
            (this IRuleBuilder<T, int> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBePrimaryEmailAddress(queries));
        }
    }

    public class MustNotBePrimaryEmailAddress : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBePrimaryEmailAddress(IProcessQueries queries)
            : base(() => Resources.Validation_EmailAddress_CannotBePrimary)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var emailAddressId = (int)context.PropertyValue;
            var entity = _queries.Execute(new EmailAddressBy(emailAddressId)).Result;
            if (entity == null || !entity.IsPrimary) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }
}
