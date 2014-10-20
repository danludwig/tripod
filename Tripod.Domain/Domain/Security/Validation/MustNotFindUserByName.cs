using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustNotFindUserByNameExtensions
    {
        /// <summary>
        /// Validates that no User exists in the underlying data store with this UserName, unless the
        /// user has the Id provided in the unlessUserHasId argument.
        /// </summary>
        /// <typeparam name="T">The command with the User Name property to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating User by Name.</param>
        /// <param name="unlessUserHasId">When this is provided, and a User is found with an Id property
        ///     matching this value, the validation result will be true.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustNotFindUserByName<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, int> unlessUserHasId = null)
        {
            return ruleBuilder.SetValidator(new MustNotFindUserByName<T>(queries, unlessUserHasId));
        }
    }

    internal class MustNotFindUserByName<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, int> _unlessUserHasId;

        internal MustNotFindUserByName(IProcessQueries queries, Func<T, int> unlessUserHasId = null)
            : base(() => Resources.Validation_AlreadyExists)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
            _unlessUserHasId = unlessUserHasId;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var userName = (string)context.PropertyValue;
            var entity = _queries.Execute(new UserBy(userName)).Result;

            if (entity != null && _unlessUserHasId != null)
            {
                var userId = _unlessUserHasId((T) context.Instance);
                if (entity.Id == userId) entity = null;
            }

            // assert that user does not exist
            return entity == null;
        }
    }
}
