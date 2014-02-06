using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeEmailAddressWithUserId<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, int> _userId; 

        internal MustBeEmailAddressWithUserId(IProcessQueries queries, Func<T, int> userId)
            : base(() => Resources.Validation_NotAuthorized_IntIdValue)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (userId == null) throw new ArgumentNullException("userId");
            _queries = queries;
            _userId = userId;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var emailAddressId = (int)context.PropertyValue;
            var userId = _userId((T) context.Instance);
            var entity = _queries.Execute(new EmailAddressBy(emailAddressId)).Result;
            if (entity == null || entity.UserId == userId) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustBeEmailAddressWithUserIdExtensions
    {
        public static IRuleBuilderOptions<T, int> MustBeEmailAddressWithUserId<T>
            (this IRuleBuilder<T, int> ruleBuilder, IProcessQueries queries, Func<T, int> userId)
        {
            return ruleBuilder.SetValidator(new MustBeEmailAddressWithUserId<T>(queries, userId));
        }
    }
}
