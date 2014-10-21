using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustFindUserByNameOrVerifiedEmailExtensions
    {
        /// <summary>
        /// Validates that an User entity with this Name or verified Email exists in the underlying data store.
        /// </summary>
        /// <typeparam name="T">The command with the name or email to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating User by Name or verified Email.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustFindUserByNameOrVerifiedEmail<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindUserByNameOrVerifiedEmail(queries));
        }
    }

    internal class MustFindUserByNameOrVerifiedEmail : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindUserByNameOrVerifiedEmail(IProcessQueries queries)
            : base(() => Resources.Validation_CouldNotFind)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var nameOrEmail = (string)context.PropertyValue;
            if (!string.IsNullOrWhiteSpace(nameOrEmail))
            {
                var user = _queries.Execute(new UserByNameOrVerifiedEmail(nameOrEmail)).Result;
                if (user != null) return true;
            }

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }
}
