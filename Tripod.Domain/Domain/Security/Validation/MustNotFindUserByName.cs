using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotFindUserByName<T> : PropertyValidator
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

    public static class MustNotFindUserByNameExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotFindUserByName<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, int> unlessUserHasId = null)
        {
            return ruleBuilder.SetValidator(new MustNotFindUserByName<T>(queries, unlessUserHasId));
        }
    }
}
