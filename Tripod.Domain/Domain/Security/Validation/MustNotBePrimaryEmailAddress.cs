using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustNotBePrimaryEmailAddressExtensions
    {
        /// <summary>
        /// Validates that the email address with this Id is not the primary email address for its User.
        /// </summary>
        /// <typeparam name="T">The command with the email address to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating EmailAddress by Id.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, int> MustNotBePrimaryEmailAddress<T>
            (this IRuleBuilder<T, int> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBePrimaryEmailAddress(queries));
        }
    }

    internal class MustNotBePrimaryEmailAddress : PropertyValidator
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
