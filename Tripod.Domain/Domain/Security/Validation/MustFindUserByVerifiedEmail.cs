using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustFindUserByVerifiedEmailExtensions
    {
        /// <summary>
        /// Validates that an User entity with this verified Email exists in the underlying data store.
        /// </summary>
        /// <typeparam name="T">The command with the email to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating User by verified Email.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustFindUserByVerifiedEmail<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindUserByVerifiedEmail(queries));
        }
    }

    internal class MustFindUserByVerifiedEmail : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindUserByVerifiedEmail(IProcessQueries queries)
            : base(() => Resources.Validation_CouldNotFind)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var email = (string)context.PropertyValue;
            if (!string.IsNullOrWhiteSpace(email))
            {
                var query = new EmailAddressBy(email)
                {
                    IsVerified = true,
                    EagerLoad = new Expression<Func<EmailAddress, object>>[]
                    {
                        x => x.User,
                    },
                };
                var emailByValue = _queries.Execute(query).Result;
                if (emailByValue != null && emailByValue.User != null) return true;
            }

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }
}
