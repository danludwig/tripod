using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustFindEmailAddressByIdExtensions
    {
        /// <summary>
        /// Validates that an EmailAddress entity with this id exists in the underlying data store.
        /// </summary>
        /// <typeparam name="T">The command with the EmailAddress Id to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating EmailAddress by Id.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, int> MustFindEmailAddressById<T>
            (this IRuleBuilder<T, int> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindEmailAddressById(queries));
        }
    }

    internal class MustFindEmailAddressById : PropertyValidator
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
}
