using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustFindUserByIdExtensions
    {
        public static IRuleBuilderOptions<T, int> MustFindUserById<T>
            (this IRuleBuilder<T, int> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindUserById(queries));
        }
    }

    internal class MustFindUserById : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindUserById(IProcessQueries queries)
            : base(() => Resources.Validation_DoesNotExist_IntIdValue)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var userId = (int)context.PropertyValue;
            var entity = _queries.Execute(new UserBy(userId)).Result;
            return entity != null;
        }
    }
}
