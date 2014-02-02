using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeEmailAddressWithOwnerId<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, int> _ownerId; 

        internal MustBeEmailAddressWithOwnerId(IProcessQueries queries, Func<T, int> ownerId)
            : base(() => Resources.Validation_NotAuthorized_IntIdValue)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (ownerId == null) throw new ArgumentNullException("ownerId");
            _queries = queries;
            _ownerId = ownerId;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var emailAddressId = (int)context.PropertyValue;
            var ownerId = _ownerId((T) context.Instance);
            var entity = _queries.Execute(new EmailAddressBy(emailAddressId)).Result;
            if (entity == null || entity.OwnerId == ownerId) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustBeEmailAddressWithOwnerIdExtensions
    {
        public static IRuleBuilderOptions<T, int> MustBeEmailAddressWithOwnerId<T>
            (this IRuleBuilder<T, int> ruleBuilder, IProcessQueries queries, Func<T, int> ownerId)
        {
            return ruleBuilder.SetValidator(new MustBeEmailAddressWithOwnerId<T>(queries, ownerId));
        }
    }
}
